
using Microsoft.EntityFrameworkCore;
using MyBackedApi.Data;
using MyBackedApi.Models;
using MyBackendApi.Repositories;

namespace MyBackedApi.Repositories
{
    public class NotificationsRepository : BaseRepository
    {
        public NotificationsRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<Notification>> GetAllAsync(Guid userId) =>
            await _context.Notifications.Where(n => n.UserId == userId).ToListAsync();

        public async Task<Notification> AddAsync(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        public async Task DeleteAsync(Guid id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Notification> UpdateAsync(Notification notification)
        {
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        public async Task<Notification> GetByIdAsync(Guid id) =>
            await _context.Notifications.FindAsync(id);
    }

}
