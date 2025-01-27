namespace MyBackedApi.Models
{
    public class Occupation
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  
    }
}
