using MyBackendApi.Models.Responses;

namespace MyBackedApi.DTOs.User.Responses
{
    public class GetUsersResponse
    {
        public List<GetUserResponse> Users { get; set; }
        public int TotalUsers { get; set; }
    }
}
