using Microsoft.EntityFrameworkCore;
using MyBackedApi.Data;
using MyBackedApi.Models;
using MyBackendApi.Repositories;

namespace MyBackedApi.Repositories
{
    public class EventsRepository : BaseRepository
    {

        public EventsRepository(ApplicationDbContext context) : base(context) {}

        public async Task<List<Event>> GetAllAsync()
        {
            return await _context.Events
                .ToListAsync();
        }

        public async Task<Event?> GetByIdAsync(Guid id)
        {
            return await _context.Events.FindAsync(id);
        }

        public async Task AddAsync(Event e)
        {
            _context.Events.Add(e);
            await _context.SaveChangesAsync();
        }

        public async Task<Event> UpdateAsync(Event e)
        {
            _context.Events.Update(e);
            await _context.SaveChangesAsync();
            return e;
        }

        public async Task DeleteAsync(Guid id)
        {
            var e = await _context.Events.FindAsync(id);
            if (e != null)
            {
                _context.Events.Remove(e);
                await _context.SaveChangesAsync();
            }
        }
    }
}
