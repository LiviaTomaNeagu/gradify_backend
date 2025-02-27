using Microsoft.EntityFrameworkCore;
using MyBackedApi.Data;
using MyBackedApi.DTOs.User.Requests;
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

        public async Task<Occupation> GetOccupationByDomain(string domain)
        {
            return await _context.Occupations
                .FirstAsync(o => o.Domain == domain);
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

        public async Task<(IEnumerable<Occupation> Occupations, int TotalOccupation)> GetOccupations(GetOccupationsRequest payload)
        {
            var coordinatorOccupation = await GetOccupationByName("PROFESOR UNITBV");
            var studentOccupation = await GetOccupationByName("STUDENT");

            if (coordinatorOccupation == null || studentOccupation == null)
            {
                throw new Exception("Required occupations not found");
            }

            var query = _context.Occupations
                .Where(o => o.Id != coordinatorOccupation.Id && o.Id != studentOccupation.Id)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(payload.SearchTerm))
            {
                string searchTerm = payload.SearchTerm.Trim().ToLower();
                query = query.Where(u =>
                    u.Name.ToLower().Contains(searchTerm) ||
                    u.Domain.ToLower().Contains(searchTerm));
            }

            var totalOccupation = await query.CountAsync();

            var skip = (payload.Page - 1) * payload.PageSize;
            var occupations = await query
                .OrderBy(u => u.Name)
                .Skip(skip)
                .Take(payload.PageSize)
                .ToListAsync();

            return (occupations, totalOccupation);
        }


        public async Task<int> GetActiveOccupationsCount()
        {
            var coordinatorOccupationId = GetOccupationByName("PROFESOR UNITBV").Result.Id;
            var studentOccupationId = GetOccupationByName("STUDENT").Result.Id;

            return await _context.Answers
                .Include(a => a.User)
                .Where(a => a.User.OccupationId != coordinatorOccupationId && a.User.OccupationId != studentOccupationId)
                .Select(a => a.User.OccupationId)
                .Distinct()
                .CountAsync();
        }

        public async Task<bool> IsOccupationActive(Guid occupationId)
        {
            return await _context.Answers
                .Include(a => a.User)
                .Where(a => a.User.OccupationId == occupationId)
                .Select(a => a.User.OccupationId)
                .Distinct()
                .AnyAsync();
        }

    }
}
