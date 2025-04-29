using MyBackedApi.Models;

namespace MyBackedApi.DTOs.User.Responses
{
    public class GetStudentResponse : StudentDetails
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string AvatarUrl { get; set; }

    }
}
