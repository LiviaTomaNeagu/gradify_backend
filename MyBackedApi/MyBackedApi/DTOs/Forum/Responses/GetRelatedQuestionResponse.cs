using MyBackedApi.Enums;

namespace MyBackedApi.DTOs.Forum.Responses
{
    public class GetRelatedQuestionResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int AnswersCount { get; set; }
        public TopicEnum Topic { get; set; }
    }
}
