using Microsoft.EntityFrameworkCore;
using MyBackedApi.Data;
using MyBackedApi.Models;
using MyBackendApi.Repositories;

namespace MyBackedApi.Repositories
{
    public class OccupationRepository : BaseRepository
    {
        public OccupationRepository(ApplicationDbContext context) : base(context)
        { }

        public async Task<Occupation> GetOccupationByIdAsync(Guid? occupationId)
        {
            var occupation = await _context.Occupations
                .FirstOrDefaultAsync(u => u.Id == occupationId);

            if (occupation == null)
            {
                throw new KeyNotFoundException("Occupation not found.");
            }

            return occupation;
        }

        public async Task<Occupation> GetOccupationByName(string name)
        {
            return await _context.Occupations
                .FirstAsync(o => o.Name == name);
        }

        public async Task<int> GetNumberOfResponsesAsync(Guid? occupationId)
        {
            var number = await _context.Answers
                .Include(u => u.User)
                .Where(a => a.User.OccupationId == occupationId)
                .CountAsync();

            return number;
        }

        public async Task<int> GetNumberOfUsersAsync(Guid? occupationId)
        {
            var number = await _context.Users
                .Where(u => u.OccupationId == occupationId)
                .CountAsync();

            return number;
        }

        public async Task<Guid> AddOccupationAsync(Occupation occupation)
        {
            await _context.Occupations.AddAsync(occupation);
            await _context.SaveChangesAsync();

            return _context.Occupations
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefault()
                .Id;
        }

        public async Task UpdateOccupationAsync(Occupation occupation)
        {
            _context.Occupations.Update(occupation);
            await _context.SaveChangesAsync();
        }

        public async Task<Guid> GetAdminForOccupation(Guid occupationId)
        {
            var user = await _context.Users
               .Where(u => u.OccupationId == occupationId && u.Role == Enums.RoleTypeEnum.ADMIN_CORPORATE)
               .FirstAsync();

            return user.Id;
        }
    }
}
