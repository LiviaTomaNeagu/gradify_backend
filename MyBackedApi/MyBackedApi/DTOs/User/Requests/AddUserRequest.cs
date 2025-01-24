using MyBackedApi.Enums;

namespace MyBackedApi.DTOs.User.Requests
{
    public class AddUserRequest
    {
        public string Username { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public RoleTypeEnum Role { get; set; }
        public Guid OccupationId { get; set; }
    }
}
