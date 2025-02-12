using MyBackedApi.DTOs.Forum.Requests;
using MyBackedApi.DTOs.Forum.Responses;
using MyBackedApi.Mappings;
using MyBackedApi.Models;
using MyBackendApi.Repositories;

namespace MyBackendApi.Services
{
    public class QuestionService
    {
        private readonly QuestionRepository _questionRepository;

        public QuestionService(QuestionRepository questionRepository)
        {
            _questionRepository = questionRepository;
        }

        public async Task<Question> AddQuestionAsync(AddQuestionRequest request)
        {
            var question = new Question
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow,
                UserId = request.UserId,
                Topic = request.Topic
            };

            await _questionRepository.AddQuestionAsync(question);
            return question;
        }

        public async Task<GetQuestionsResponse> GetAllQuestionsAsync(GetQuestionsRequest payload)
        {
            var (questions, totalQuestions) = await _questionRepository.GetAllQuestionsAsync(payload);

            var response = new GetQuestionsResponse
            {
                Questions = questions.ToGetQuestionResponses(),
                totalQuestions = totalQuestions
            };
            return response;
        }

        public async Task<GetQuestionResponse?> GetQuestionByIdAsync(Guid id)
        {
            var question = await _questionRepository.GetQuestionByIdAsync(id);
            return question?.ToGetQuestionResponse();
        }

        public async Task<GetQuestionDetailsResponse?> GetQuestionDetails(Guid id)
        {
            var question = await _questionRepository.GetQuestionByIdAsync(id);
            return question?.ToGetQuestionDetailsResponse();
        }

        public async Task<List<GetRelatedQuestionResponse>> GetRelatedQuestionsAsync(GetRelatedQuestionRequest payload)
        {
            var stopWords = new HashSet<string>
            {
                "how", "and", "is", "done", "the", "a", "an", "in", "on", "at", "to", "for", "with", "by", "of", "this", "that", "it", "from"
            };

            var searchWords = payload.Content
                .Split(new[] { ' ', ',', '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(w => w.ToLower())
                .Where(w => !stopWords.Contains(w))
                .ToList();

            var questions = await _questionRepository.GetRelatedQuestionsAsync(searchWords);

            if (questions == null || !questions.Any())
            {
                return new List<GetRelatedQuestionResponse>();
            }

            return questions.Select(q => q.ToRelatedQuestionResponse()).ToList();
        }

    }
}
