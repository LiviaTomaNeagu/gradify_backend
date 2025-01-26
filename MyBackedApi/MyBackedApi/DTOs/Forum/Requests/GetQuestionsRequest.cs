using MyBackedApi.Enums;

namespace MyBackedApi.DTOs.Forum.Requests
{
    public class GetQuestionsRequest
    {
        public string Search { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public TopicEnum? Topic { get; set; }
    }
}
