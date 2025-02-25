using MyBackedApi.Enums;

namespace MyBackedApi.DTOs.Auth.Requests
{
    public class RegisterUserRequest
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public RoleTypeEnum Role { get; set; }

    }
}
