namespace MyBackedApi.Models
{
    public class Student_Coordinator
    {
        public Guid StudentId { get; set; }
        public User Student { get; set; }

        public Guid CoordinatorId { get; set; }
        public User Coordinator { get; set; }
    }
}
