using MyBackedApi.Enums;
using MyBackedApi.Models;

namespace MyBackedApi.DTOs.User
{
    public class ShortUserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public RoleTypeEnum Role { get; set; }
        public bool IsApproved { get; set; }
        public string AvatarUrl { get; set; }

        public Guid? OccupationId { get; set; }
        public Occupation Occupation { get; set; }

    }
}
