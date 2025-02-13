using System.ComponentModel.DataAnnotations;

namespace MyBackedApi.Models
{
    public class BaseModel
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
