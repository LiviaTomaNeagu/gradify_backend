using MyBackedApi.DTOs.Forum.Requests;
using MyBackedApi.DTOs.Forum.Responses;
using MyBackedApi.Mappings;
using MyBackedApi.Models;
using MyBackendApi.Mappings;
using MyBackendApi.Repositories;
using System.Linq;

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

        public async Task<GetQuestionsResponse> GetAllQuestionsAsync()
        {
            var questions =  await _questionRepository.GetAllQuestionsAsync();

            var response = new GetQuestionsResponse
            {
                Questions = questions.ToGetQuestionResponses(),
                totalQuestions = questions.Count(),
                totalFilteredQuestions = questions.Count()
            };
            return response;
        }

        public async Task<GetQuestionResponse?> GetQuestionByIdAsync(Guid id)
        {
            var question =  await _questionRepository.GetQuestionByIdAsync(id);
            return question?.ToGetQuestionResponse();
        }
    }
}
