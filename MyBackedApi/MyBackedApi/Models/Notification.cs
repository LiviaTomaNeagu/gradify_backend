using MyBackedApi.Enums;

namespace MyBackedApi.Models
{
    public class Notification : BaseModel
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public bool Read { get; set; }
        public NotificationTypeEnum Type { get; set; }
        public string Route { get; set; }
        public Guid UserId { get; set; }
    }
}
