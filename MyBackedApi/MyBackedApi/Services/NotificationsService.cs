using Microsoft.AspNetCore.Mvc;
using MyBackedApi.DTOs.Notifications.Requests;
using MyBackedApi.DTOs.Notifications.Responses;
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
            await _repo.AddAsync(notification);
            return notification;
        }

        internal async Task ReadNotificationAsync(Guid notificationId)
        {
            var notification = await _repo.GetByIdAsync(notificationId);
            notification.Read = true;
            await _repo.SaveChangesAsync();
        }

        public async Task<ActionResult<UnreadCountResponse>> UnreadCountAsync(Guid userId)
        {
            var notifications = await _repo.GetAllAsync(userId);
            return new UnreadCountResponse()
            {
                TotalUnread = notifications
                .Where(n => n.Read == false)
                .Count()
            };
        }
    }
}
