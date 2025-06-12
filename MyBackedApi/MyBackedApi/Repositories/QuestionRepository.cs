using Microsoft.EntityFrameworkCore;
using MyBackedApi.Data;
using MyBackedApi.DTOs.Forum.Requests;
using MyBackedApi.DTOs.Forum.Responses;
using MyBackedApi.Enums;
using MyBackedApi.Helpers;
using MyBackedApi.Models;
using MyBackedApi.Services;
using System.Text;

namespace MyBackendApi.Repositories
{
    public class QuestionRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IOpenAiService _openAiService;
        private readonly S3Service _s3Service;

        public QuestionRepository(ApplicationDbContext context, IOpenAiService openAiService, S3Service s3Service)
        {
            _context = context;
            _openAiService = openAiService;
            _s3Service = s3Service;
        }

        public async Task AddQuestionAsync(Question question)
        {
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
        }

        public async Task AddQuestionDocumentsAsync(Question question)
        {

            var attachments = await _s3Service.GetQuestionAttachmentsAsync(question.Id);
            if (attachments.Any())
            {
                var extractedFiles = await FileTextExtractor.ExtractTextFromFilesAsync(attachments);

                // Vechea funcționalitate: reconstruiești ImageText și DocumentText pentru compatibilitate
                var imageText = new StringBuilder();
                var documentText = new StringBuilder();

                foreach (var f in extractedFiles)
                {
                    var ext = Path.GetExtension(f.FileName).ToLower();
                    if ((ext == ".png" || ext == ".jpg" || ext == ".jpeg") && !string.IsNullOrWhiteSpace(f.FullText))
                        imageText.AppendLine(f.FullText);
                    else if (ext == ".pdf")
                    {
                        foreach (var p in f.Pages)
                        {
                            documentText.AppendLine(p.Text);
                        }
                        if (!string.IsNullOrWhiteSpace(f.FullText))
                            documentText.AppendLine(f.FullText);
                    }
                }

                question.ImageText = imageText.ToString();
                question.DocumentText = documentText.ToString();

                await _s3Service.SaveMetadataJsonAsync(question.Id, extractedFiles);
            }


            if (!string.IsNullOrWhiteSpace(question.Title))
                question.TitleEmbedding = await _openAiService.GetEmbeddingAsync(question.Title);

            if (!string.IsNullOrWhiteSpace(question.Content))
                question.ContentEmbedding = await _openAiService.GetEmbeddingAsync(question.Content);

            if (!string.IsNullOrWhiteSpace(question.ImageText))
                question.ImageEmbedding = await _openAiService.GetEmbeddingAsync(question.ImageText);

            //var fullText = TextPreprocessor.Preprocess(question);
            //question.EmbeddingVector = await _openAiService.GetEmbeddingAsync(fullText);

            _context.Questions.Update(question);
            await _context.SaveChangesAsync();
        }


        public async Task<(IEnumerable<Question> Questions, int TotalQuestions)> GetAllQuestionsAsync(GetQuestionsRequest payload)
        {
            var query = _context.Questions
                .Include(q => q.Answers)
                .Include(q => q.User)
                    .ThenInclude(u => u.Occupation)
                .OrderByDescending(q => q.CreatedAt)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(payload.Search))
            {
                query = query.Where(q => q.Title.Contains(payload.Search) || q.Content.Contains(payload.Search));
            }

            if (payload.Topics != null && payload.Topics.Any())
            {
                query = query.Where(q => payload.Topics.Contains(q.Topic));
            }

            var totalQuestions = await query.CountAsync();

            var skip = (payload.Page - 1) * payload.PageSize;
            var questions = await query.Skip(skip).Take(payload.PageSize)
                .ToListAsync();

            return (questions, totalQuestions);
        }

        public async Task<List<Question>> GetAllQuestionsForSimilarityAsync()
        {
            return await _context.Questions
                .Select(q => new Question
                {
                    Id = q.Id,
                    Title = q.Title,
                    Content = q.Content,
                    Topic = q.Topic
                })
                .ToListAsync();
        }



        public async Task<Question?> GetQuestionByIdAsync(Guid id)
        {
            var question = await _context.Questions
                .Include(q => q.Answers)
                    .ThenInclude(a => a.User)
                        .ThenInclude(u => u.Occupation)
                .Include(q => q.User)
                    .ThenInclude(u => u.Occupation)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question != null)
            {
                question.Answers = question.Answers.OrderBy(a => a.CreatedAt).ToList();
            }

            return question;
        }

        public async Task<List<Question>> GetRelatedQuestionsAsync(List<string>? searchWords)
        {
            return await _context.Questions
                .Where(q => searchWords.Any(word => q.Content.ToLower().Contains(word)))
                .Include(q => q.Answers)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<User>> GetUsersInteractedWith(Guid mentorId)
        {
            return await _context.Questions
                .Include(q => q.User)
                    .ThenInclude(u => u.Occupation)
                .Where(q => q.Answers.Any(a => a.UserId == mentorId) && q.UserId != mentorId)
                .OrderByDescending(q => q.CreatedAt)
                .Select(q => q.User)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<Question>> GetLatestQuestions(Guid mentorId)
        {
            return await _context.Questions
                .Where(q => q.Answers.Any(a => a.UserId == mentorId))
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<TopicEnum>> GetFavoriteTopic(Guid mentorId)
        {
            return await _context.Questions
                .Where(q => q.Answers.Any(a => a.UserId == mentorId))
                .GroupBy(q => q.Topic)
                .Select(group => new
                {
                    Topic = group.Key,
                    Count = group.Count(),
                    LatestAnsweredAt = group.Max(q => q.Answers
                        .Where(a => a.UserId == mentorId)
                        .Max(a => a.CreatedAt))
                })
                .OrderByDescending(g => g.Count)
                .ThenByDescending(g => g.LatestAnsweredAt)
                .Select(g => g.Topic)
                .ToListAsync();
        }

        public async Task<List<User>> GetMentorsInteractedWith(Guid studentId)
        {
            var answers = await _context.Questions
                .Where(q => q.UserId == studentId)
                .SelectMany(q => q.Answers)
                .Include(a => a.User)
                    .ThenInclude(u => u.Occupation)
                .ToListAsync();

            var mentors = answers
                .Where(a => a.UserId != studentId)
                .Select(a => a.User)
                .Distinct()
                .ToList();

            return mentors;
        }


        public async Task<List<Question>> GetLatestQuestionsByStudent(Guid studentId)
        {
            return await _context.Questions
                .Where(q => q.UserId == studentId)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<TopicEnum>> GetFavoriteTopicsByStudent(Guid studentId)
        {
            return await _context.Questions
                .Where(q => q.UserId == studentId)
                .GroupBy(q => q.Topic)
                .Select(group => new
                {
                    Topic = group.Key,
                    Count = group.Count(),
                    LatestAskedAt = group.Max(q => q.CreatedAt)
                })
                .OrderByDescending(g => g.Count)
                .ThenByDescending(g => g.LatestAskedAt)
                .Select(g => g.Topic)
                .ToListAsync();
        }

        public async Task<List<SmartSearchResultDto>> GetQuestionsBySemanticSimilarity(string searchText)
        {
            var queryEmbedding = await _openAiService.GetEmbeddingAsync(searchText);

            var questions = await _context.Questions
                .Where(q =>
                    (q.TitleEmbedding != null && q.TitleEmbedding.Length > 0) ||
                    (q.ContentEmbedding != null && q.ContentEmbedding.Length > 0) ||
                    (q.ImageEmbedding != null && q.ImageEmbedding.Length > 0) ||
                    (q.DocumentEmbedding != null && q.DocumentEmbedding.Length > 0))
                .Include(q => q.Answers)
                .Include(q => q.User)
                .ToListAsync();

            var results = new List<SmartSearchResultDto>();

            // Ponderi pentru fiecare sursă
            var weights = new Dictionary<string, double>
    {
        { "title", 1.0 },
        { "content", 1.025 },
        { "image", 1.0 },
        { "document", 1.3 } // Boost implicit pentru documente
    };

            foreach (var question in questions)
            {
                var scores = new List<(string Source, float[] Embedding, string Text)>
        {
            ("title", question.TitleEmbedding, question.Title ?? ""),
            ("content", question.ContentEmbedding, question.Content ?? ""),
            ("image", question.ImageEmbedding, question.ImageText ?? "")
        };

                string bestSource = "";
                string bestSnippet = "";
                double bestScore = 0;

                // 1️⃣ Calculează scorurile embeddings
                foreach (var (source, embedding, text) in scores)
                {
                    if (embedding == null || embedding.Length == 0 || string.IsNullOrWhiteSpace(text))
                        continue;

                    var score = SimilarityFunctions.CombinedSimilarity(
                        queryEmbedding,
                        embedding,
                        searchText,
                        text
                    );

                    // Aplică pondere
                    score *= weights.GetValueOrDefault(source, 1.0);

                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestSource = source;
                        bestSnippet = text.Length > 200 ? text.Substring(0, 200) + "..." : text;
                    }
                }

                // 2️⃣ Căutare concretă în metadata.json pentru document
                string? fileName = null;
                int? page = null;

                var metadata = await _s3Service.GetMetadataForQuestionAsync(question.Id);
                if (metadata != null)
                {
                    foreach (var file in metadata)
                    {
                        var pages = file.Value;
                        foreach (var p in pages)
                        {
                            if (DocumentMatch.FuzzyMatch(p.Value, searchText))
                            {
                                fileName = file.Key;
                                page = int.Parse(p.Key);
                                bestSnippet = FileTextExtractor.ExtractSnippet(p.Value, searchText);

                                bestScore += 0.2;

                                bestSource = "document"; // optional, dacă vrei să-l forțezi

                                break;
                            }

                        }
                    }
                }

                results.Add(new SmartSearchResultDto
                {
                    Id = question.Id,
                    Title = question.Title,
                    Content = question.Content,
                    Score = bestScore,
                    MatchedSource = bestSource,
                    MatchedSnippet = bestSnippet,
                    Topic = question.Topic,
                    FileName = fileName,
                    Page = page
                });
            }

            return results
                .OrderByDescending(r => r.Score)
                .Take(9)
                .ToList();
        }

        //Embedded Controller for populatig existing questions
        public async Task GenerateEmbeddingsForAllQuestionsAsync()
        {
            var questions = await _context.Questions
                .Include(q => q.Answers)
                .ToListAsync();

            foreach (var question in questions)
            {
                // 1. Title
                if (!string.IsNullOrWhiteSpace(question.Title))
                {
                    question.TitleEmbedding = await _openAiService.GetEmbeddingAsync(question.Title);
                }

                // 2. Content
                if (!string.IsNullOrWhiteSpace(question.Content))
                {
                    question.ContentEmbedding = await _openAiService.GetEmbeddingAsync(question.Content);
                }

                // 3. ImageText
                if (!string.IsNullOrWhiteSpace(question.ImageText))
                {
                    question.ImageEmbedding = await _openAiService.GetEmbeddingAsync(question.ImageText);
                }

                // 4. DocumentText
                if (!string.IsNullOrWhiteSpace(question.DocumentText))
                {
                    question.DocumentEmbedding = await _openAiService.GetEmbeddingAsync(question.DocumentText);
                }

                // 5. Global vector (combinat, fallback)
                var fullText = TextPreprocessor.Preprocess(question);
                question.EmbeddingVector = await _openAiService.GetEmbeddingAsync(fullText);
            }

            await _context.SaveChangesAsync();
        }



        public async Task PopulateTextFromFilesForQuestionsAsync()
        {
            var questions = await _context.Questions.ToListAsync();

            foreach (var question in questions)
            {
                var files = await _s3Service.GetQuestionAttachmentsAsync(question.Id);

                if (files == null || files.Count == 0)
                    continue;

                var extractedFiles = await FileTextExtractor.ExtractTextFromFilesAsync(files);

                var imageText = new StringBuilder();
                var documentText = new StringBuilder();

                foreach (var f in extractedFiles)
                {
                    var ext = Path.GetExtension(f.FileName).ToLower();
                    if ((ext == ".png" || ext == ".jpg" || ext == ".jpeg") && !string.IsNullOrWhiteSpace(f.FullText))
                        imageText.AppendLine(f.FullText);
                    else if (ext == ".pdf")
                    {
                        foreach (var p in f.Pages)
                        {
                            documentText.AppendLine(p.Text);
                        }
                        if (!string.IsNullOrWhiteSpace(f.FullText))
                            documentText.AppendLine(f.FullText);
                    }
                }

                question.ImageText = imageText.ToString();
                question.DocumentText = documentText.ToString();

                await _s3Service.SaveMetadataJsonAsync(question.Id, extractedFiles);
            }

            await _context.SaveChangesAsync();
        }



    }
}
