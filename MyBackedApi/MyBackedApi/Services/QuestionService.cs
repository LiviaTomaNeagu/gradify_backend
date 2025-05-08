using MyBackedApi.DTOs.Forum.Requests;
using MyBackedApi.DTOs.Forum.Responses;
using MyBackedApi.Extensions;
using MyBackedApi.Mappings;
using MyBackedApi.Models;
using MyBackedApi.Services;
using MyBackendApi.Repositories;

namespace MyBackendApi.Services
{
    public class QuestionService
    {
        private readonly QuestionRepository _questionRepository;
        private readonly QuestionSimilarityService _similarityService;

        public QuestionService(QuestionRepository questionRepository, QuestionSimilarityService questionSimilarityService)
        {
            _questionRepository = questionRepository;
            _similarityService = questionSimilarityService;
        }

        public async Task<Question> AddQuestionAsync(AddQuestionRequest request, Guid currentUserId)
        {
            var question = new Question
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Content = request.DescriptionHtml != null ? request.DescriptionHtml : string.Empty,
                CreatedAt = DateTime.UtcNow,
                UserId = currentUserId,
                Topic = request.Topic.KeyToTopicEnum()
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
            var allQuestions = await _questionRepository.GetAllQuestionsForSimilarityAsync();

            _similarityService.LoadQuestions(allQuestions);

            var similar = _similarityService.GetSimilarQuestions(payload.Content)
                .Where(q => q.Score > 0.05)
                .Take(10)
                .Select(q => q.Id)
                .ToList();

            var finalQuestions = allQuestions
                .Where(q => similar.Contains(q.Id))
                .ToList();

            return finalQuestions.Select(q => q.ToRelatedQuestionResponse()).ToList();

        }

    }
}
