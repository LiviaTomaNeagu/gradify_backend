using MyBackedApi.DTOs.User;
using MyBackedApi.Enums;

namespace MyBackedApi.DTOs.Forum
{
    public class LatestQuestion
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public TopicEnum Topic { get; set; }
        public ShortUserDto Author { get; set; }


    }
}
