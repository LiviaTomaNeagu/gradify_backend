using MyBackedApi.Enums;

namespace MyBackedApi.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public RoleTypeEnum Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int CompletedSteps { get; set; }

        public Guid? OccupationId { get; set; }
        public Occupation Occupation { get; set; }

        public ICollection<Question> Questions { get; set; }
    }
}
