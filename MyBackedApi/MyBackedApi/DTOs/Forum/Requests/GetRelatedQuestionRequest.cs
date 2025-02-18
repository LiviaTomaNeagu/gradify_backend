using MyBackedApi.Enums;

namespace MyBackedApi.DTOs.Forum.Requests
{
    public class GetRelatedQuestionRequest
    {
        public string Content { get; set; }
        public TopicEnum Topic { get; set; }
    }
}
