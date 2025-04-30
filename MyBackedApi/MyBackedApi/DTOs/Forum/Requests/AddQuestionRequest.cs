using MyBackedApi.Enums;

namespace MyBackedApi.DTOs.Forum.Requests
{
    public class AddQuestionRequest
    {
        public string Title { get; set; }
        public string? DescriptionHtml { get; set; }
        public string Topic { get; set; }
    }

}
