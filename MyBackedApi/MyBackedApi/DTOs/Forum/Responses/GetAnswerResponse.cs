using MyBackedApi.Models;

namespace MyBackedApi.DTOs.Forum.Responses
{
    public class GetAnswerResponse
    {
        public Guid Id { get; set; }
        public Guid QuestionId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid UserId { get; set; }
    }
}
