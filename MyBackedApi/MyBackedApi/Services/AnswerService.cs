using MyBackedApi.DTOs.Forum.Requests;
using MyBackedApi.DTOs.Forum.Responses;
using MyBackedApi.Models;
using MyBackendApi.Mappings;
using MyBackendApi.Repositories;

namespace MyBackendApi.Services
{
    public class AnswerService
    {
        private readonly AnswerRepository _answerRepository;
        private readonly QuestionRepository _questionRepository;

        public AnswerService(AnswerRepository answerRepository, QuestionRepository questionRepository)
        {
            _answerRepository = answerRepository;
            _questionRepository = questionRepository;
        }

        public async Task<Answer> AddAnswerAsync(Guid questionId, AddAnswerRequest request)
        {
            var question = await _questionRepository.GetQuestionByIdAsync(questionId);
            if (question == null)
            {
                throw new KeyNotFoundException("Question not found");
            }

            var answer = new Answer
            {
                Id = Guid.NewGuid(),
                QuestionId = questionId,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow,
                UserId = request.UserId
            };

            await _answerRepository.AddAnswerAsync(answer);
            return answer;
        }
    }
}
