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
        private readonly UserRepository _userRepository;

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

            var user = await _userRepository.GetUserByIdAsync(request.UserId);

            var answer = new Answer
            {
                Id = Guid.NewGuid(),
                QuestionId = questionId,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow,
                UserId = request.UserId,
                User = user
            };

            await _answerRepository.AddAnswerAsync(answer);
            return answer;
        }

        public async Task<List<GetAnswerResponse>> GetAllAnswersForQuestionAsync(Guid questionId)
        {
            var answers =  await _answerRepository.GetAllAnswersForQuestionAsync(questionId);
            return answers.ToGetAnswerResponses();
        }
    }
}
