namespace MyBackedApi.DTOs.User.Requests
{
    public class GetOccupationsRequest
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; }
    }
}
