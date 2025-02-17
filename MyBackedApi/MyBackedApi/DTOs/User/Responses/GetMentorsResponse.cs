using MyBackendApi.Models.Responses;

namespace MyBackedApi.DTOs.User.Responses
{
    public class GetMentorsResponse
    {
        public List<GetUserResponse> Mentors { get; set; }
        public int TotalUsers { get; set; }
    }
}
