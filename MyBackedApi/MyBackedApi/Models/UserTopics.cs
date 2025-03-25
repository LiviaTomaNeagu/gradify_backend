using System;
using MyBackedApi.Enums;

namespace MyBackedApi.Models
{
    public class UserTopics : BaseModel
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public TopicEnum Topic { get; set; }
    }
}
