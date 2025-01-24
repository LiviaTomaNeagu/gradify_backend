using MyBackedApi.Enums;
using MyBackedApi.Models;

namespace MyBackedApi.DTOs.Forum.Responses
{
    public class GetQuestionResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public TopicEnum Topic { get; set; }

        public List<GetAnswerResponse> Answers { get; set; }
    }
}
