using MyBackedApi.Enums;

namespace MyBackedApi.DTOs.Auth.Responses
{
    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public RoleTypeEnum Role { get; set; }

    }
}
