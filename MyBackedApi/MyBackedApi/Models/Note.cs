namespace MyBackedApi.Models
{
    public class Note : BaseModel
    {
        public string Title { get; set; }
        public string Color { get; set; }
        public DateTime Datef { get; set; }

        public Guid StudentId { get; set; }
        public User Student { get; set; }
    }

}
