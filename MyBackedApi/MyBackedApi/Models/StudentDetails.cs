using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MyBackedApi.Models
{
    public class StudentDetails : BaseModel
    {
        public string Faculty { get; set; } = string.Empty;

        public string Specialization { get; set; } = string.Empty;

        public string Group { get; set; } = string.Empty;

        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
