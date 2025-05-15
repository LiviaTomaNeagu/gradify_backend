using MyBackedApi.Enums;

namespace MyBackedApi.DTOs.Notifications.Requests
{
    public class CreateNotificationRequest
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public NotificationTypeEnum Type { get; set; }
        public string Route { get; set; }
        public Guid UserId { get; set; }
    }
}
