using MyBackedApi.DTOs.Notifications.Requests;
using MyBackedApi.Models;
using MyBackedApi.Repositories;

namespace MyBackedApi.Services
{
    public class NotificationsService
    {
        private readonly NotificationsRepository _repo;
        
        public NotificationsService(NotificationsRepository repository)
        {
            _repo = repository;
        }

        public async Task<List<Notification>> GetNotificationsAsync(Guid studentId)
        {
            var notifications = await _repo.GetAllAsync(studentId);
            return notifications.Select(n => new Notification
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                Read = n.Read,
                Type = n.Type,
                UserId = n.UserId,
                CreatedAt = n.CreatedAt,
                Route = n.Route
            })
            .OrderByDescending(n => n.CreatedAt)
            .ToList();
        }

        public async Task<Notification> CreateNotificationAsync(CreateNotificationRequest dto)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Message = dto.Message,
                Type = dto.Type,
                CreatedAt = DateTime.UtcNow,
                UserId = dto.UserId,
                Read = false,
                Route = dto.Route
            };
            //await _repo.AddAsync(notification);
            return notification;
        }


    }
}
