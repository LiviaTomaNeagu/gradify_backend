using Microsoft.EntityFrameworkCore;
using MyBackedApi.Data;
using MyBackedApi.DTOs.Forum.Requests;
using MyBackedApi.Enums;
using MyBackedApi.Helpers;
using MyBackedApi.Models;
using MyBackedApi.Services;
using System.Globalization;
using System.Text;

namespace MyBackendApi.Repositories
{
    public class QuestionRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IOpenAiService _openAiService;

        public QuestionRepository(ApplicationDbContext context, IOpenAiService openAiService)
        {
            _context = context;
            _openAiService = openAiService;
        }

        public async Task AddQuestionAsync(Question question)
        {
            _context.Questions.Add(question);
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


        public async Task GenerateEmbeddingForExistingQuestions()
        {
            var questions = await _context.Questions
                .Include(q => q.Answers)
                .Where(q => q.EmbeddingVector == null)
                .ToListAsync();

            foreach (var question in questions)
            {
                var fullText = TextPreprocessor.Preprocess(question);
                var embedding = await _openAiService.GetEmbeddingAsync(fullText);

                question.EmbeddingVector = embedding;

                // Optional: salvează și textul agregat dacă nu ai deja
                question.ImageText ??= "";
                question.DocumentText ??= "";
            }

            await _context.SaveChangesAsync();
        }

    }
}
