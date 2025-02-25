using Microsoft.EntityFrameworkCore;
using MyBackedApi.Data;
using MyBackedApi.DTOs.User.Requests;
using MyBackedApi.Enums;
using MyBackedApi.Models;
using MyBackendApi.Models.Responses;

namespace MyBackendApi.Repositories
{
    public class UserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.Occupation)
                .ToListAsync();
        }

        public async Task<(IEnumerable<User> Mentors, int TotalUsers)> GetUsersWithOccupation(GetMentorsRequest payload)
        {
            var query = _context.Users
                .Include(u => u.Occupation)
                .Where(u => u.OccupationId == payload.AdminUser.OccupationId)
                .OrderBy(u => u.Surname)
                .AsQueryable();

            var totalUsers = await query.CountAsync();

            var skip = (payload.Page - 1) * payload.PageSize;
            var users = await query.Skip(skip).Take(payload.PageSize)
                .ToListAsync();

            return (users, totalUsers);
        }

        public async Task<(IEnumerable<User> Admins, int TotalUsers)> GetAdminsCorporateAsync(GetAdminsCorporateRequest payload)
        {
            var query = _context.Users
                .Include(u => u.Occupation)
                .Where(u => u.Role == RoleTypeEnum.ADMIN_CORPORATE)
                .OrderBy(u => u.Surname)
                .AsQueryable();

            var totalUsers = await query.CountAsync();

            var skip = (payload.Page - 1) * payload.PageSize;
            var users = await query.Skip(skip).Take(payload.PageSize)
                .ToListAsync();

            return (users, totalUsers);
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            var user = await _context.Users
                .Include(u => u.Occupation)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            return user;
        }


        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }


        public async Task DeleteUserAsync(Guid id)
        {
            var user = await GetUserByIdAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetActiveUserByEmail(string email)
        {
            return await _context.Users
               .FirstOrDefaultAsync(u => u.Email == email && u.IsApproved == true);
        }

        

        public async Task ApproveUserAsync(Guid id)
        {
            var user = await GetUserByIdAsync(id);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            user.IsApproved = true;
            await _context.SaveChangesAsync();
        }

        public async Task ApproveUserAsync(string email)
        {
            var user = await GetUserByEmailAsync(email);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            user.IsApproved = true;
            await _context.SaveChangesAsync();
        }

        public async Task<(IEnumerable<User> Users, int TotalUsers)> GetUsersByRole(GetUsersForRoleRequest payload)
        {
            var query = _context.Users
                .Include(u => u.Occupation)
                .Where(u => u.Role == payload.Role)
                .OrderBy(u => u.Surname)
                .AsQueryable();

            var totalUsers = await query.CountAsync();

            var skip = (payload.Page - 1) * payload.PageSize;
            var users = await query.Skip(skip).Take(payload.PageSize)
                .ToListAsync();

            return (users, totalUsers);
        }
    }
}
