using MyBackedApi.Enums;

namespace MyBackendApi.Models.Responses
{
    public class GetUserResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public RoleTypeEnum Role { get; set; }
        public int CompletedSteps { get; set; }
    }
}

