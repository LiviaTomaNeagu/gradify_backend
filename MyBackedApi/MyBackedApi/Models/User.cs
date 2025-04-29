using MyBackedApi.Enums;

namespace MyBackedApi.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public RoleTypeEnum Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int CompletedSteps { get; set; }
        public bool IsApproved { get; set; }
        public string AvatarUrl { get; set; }

        public Guid? OccupationId { get; set; }
        public Occupation Occupation { get; set; }

        public ICollection<Question> Questions { get; set; }
        public ICollection<Student_Coordinator> Students { get; set; }  // Coordonatorul are studenți
        public ICollection<Student_Coordinator> Coordinators { get; set; } // Studentul are coordonatori
        public StudentDetails StudentDetails { get; set; }

        public ICollection<UserTopics> UserTopics { get; set; } = new List<UserTopics>();
    }
}
