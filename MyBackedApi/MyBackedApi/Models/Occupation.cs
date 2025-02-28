namespace MyBackedApi.Models
{
    public class Occupation
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Domain { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  
        public string AdminEmail { get; set; }
        public string AdminName { get; set; }
        public string AdminSurname { get; set; }
    }
}
