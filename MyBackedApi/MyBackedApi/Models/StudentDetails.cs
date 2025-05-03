using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MyBackedApi.Models
{
    public class StudentDetails : BaseModel
    {
        public string Faculty { get; set; } = string.Empty;

        public string Specialization { get; set; } = string.Empty;

        public Guid? GroupId { get; set; }
        public Group? Group { get; set; }

        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
        public string Kanban { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
