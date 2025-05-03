using MyBackedApi.Enums;
using MyBackedApi.Models;

namespace MyBackedApi.DTOs.User.Responses
{
    public class GetAccountDetailsResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public RoleTypeEnum Role { get; set; }
        public int TotalDays { get; set; }
        public string OccupationName { get; set; }
        public int Interactions { get; set; }
        public int UsersInteractedWith { get; set; }
        public List<TopicEnum> Topics { get; set; }
    }
}
