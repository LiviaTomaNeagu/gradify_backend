using MyBackedApi.DTOs.Forum.Responses;
using MyBackedApi.Models;
using MyBackendApi.Mappings;

namespace MyBackedApi.Mappings
{
    public static class QuestionMappingExtensions
    {
        public static GetQuestionResponse ToGetQuestionResponse(this Question question)
        {
            return new GetQuestionResponse
            {
                Id = question.Id,
                Title = question.Title,
                Content = question.Content,
                CreatedAt = question.CreatedAt,
                UserId = question.UserId,
                Topic = question.Topic,
                Name = question.User.Name,
                Surname = question.User.Surname,
                OccupationName = question.User.Occupation.Name
            };
        }

        public static List<GetQuestionResponse> ToGetQuestionResponses(this IEnumerable<Question> questions)
        {
            return questions.Select(q => q.ToGetQuestionResponse()).ToList();
        }

    }
}
