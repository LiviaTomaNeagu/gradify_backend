using Microsoft.EntityFrameworkCore;
using MyBackedApi.Data;

namespace MyBackendApi.Repositories
{
    public class BaseRepository
    {
        protected ApplicationDbContext _context { get; set; }

        public BaseRepository(ApplicationDbContext context)
        {
            this._context = context;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

