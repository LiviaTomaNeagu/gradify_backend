using MyBackedApi.Enums;

namespace MyBackedApi.DTOs.Forum.Responses
{
    public class SmartSearchResultDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public double Score { get; set; }
        public string MatchedSource { get; set; } // ex: title, content, image, document
        public string MatchedSnippet { get; set; } // textul concret unde a fost găsit
        public TopicEnum Topic { get; set; }
    }

}
