using MyBackedApi.Enums;

namespace MyBackedApi.DTOs.Forum.Requests
{
    public class AddQuestionRequest
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid UserId { get; set; }
        public TopicEnum Topic { get; set; }
    }

}
