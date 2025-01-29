using MyBackedApi.DTOs.Forum.Responses;
using MyBackedApi.Models;
using System.Collections.Generic;
using System.Linq;

namespace MyBackendApi.Mappings
{
    public static class AnswerMappingExtensions
    {
        public static GetAnswerResponse ToGetAnswerResponse(this Answer answer)
        {
            if (answer == null)
            {
                return null;
            }

            return new GetAnswerResponse { 
                Id = answer.Id,
                Content = answer.Content,
                CreatedAt = answer.CreatedAt,
                QuestionId = answer.QuestionId,
                Name = answer.User.Name,
                Surname = answer.User.Surname,
                OccupationName = answer.User.Occupation.Name
            };
        }

        public static List<GetAnswerResponse> ToGetAnswerResponses(this IEnumerable<Answer> answers)
        {
            if (answers == null || !answers.Any())
            {
                return new();
            }

            return answers.Select(a => a.ToGetAnswerResponse()).ToList();
        }
    }
}
