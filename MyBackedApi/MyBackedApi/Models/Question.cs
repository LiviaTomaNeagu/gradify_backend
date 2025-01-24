using MyBackedApi.Enums;

namespace MyBackedApi.Models
{
    public class Question
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid UserId { get; set; }
        public TopicEnum Topic { get; set; }

        public ICollection<Answer> Answers { get; set; }
        public User User { get; set; }
    }

}
