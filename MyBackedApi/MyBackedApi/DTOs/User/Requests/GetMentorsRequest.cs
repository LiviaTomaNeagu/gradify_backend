namespace MyBackedApi.DTOs.User.Requests
{
    public class GetMentorsRequest
    {
        public MyBackedApi.Models.User AdminUser { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

    }
}
