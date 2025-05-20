namespace MyBackedApi.DTOs.Forum.Responses
{
    public class SmartSearchResultDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public double Score { get; set; }
    }

}
