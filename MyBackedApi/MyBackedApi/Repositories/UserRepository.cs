using Microsoft.EntityFrameworkCore;
using MyBackedApi.Data;
using MyBackedApi.DTOs.User.Requests;
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

        public async Task AddOccupationAsync(Occupation occupation)
        {
            await _context.Occupations.AddAsync(occupation);
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .FirstAsync(u => u.Email == email);
        }

        public async Task<Occupation> GetOccupationByName(string name)
        {
            return await _context.Occupations
                .FirstAsync(o => o.Name == name);
        }

        public async Task<int> GetCodeForUser(string email)
        {
            var code = await _context.ActivationCodes.FirstOrDefaultAsync(ac => ac.Email == email);
            return code.Code;
        }

        public async Task DeleteCodeForUser(string email)
        {
            var user = await _context.ActivationCodes.FirstOrDefaultAsync(ac => ac.Email == email);
            if (user != null)
            {
                _context.ActivationCodes.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
