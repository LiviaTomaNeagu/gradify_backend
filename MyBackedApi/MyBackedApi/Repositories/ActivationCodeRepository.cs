using Microsoft.EntityFrameworkCore;
using MyBackedApi.Data;
using MyBackedApi.Models;
using MyBackendApi.Repositories;
using System;

namespace MyBackedApi.Repositories
{
    public class ActivationCodeRepository : BaseRepository
    {
        public ActivationCodeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<ActivationCode> GetValidActivationCodeAsync(string email, string code)
        {
            return await _context.ActivationCodes
                .FirstOrDefaultAsync(ac => ac.Email == email && ac.Code == code && ac.Expiration > DateTime.UtcNow);
        }

        public async Task AddActivationCodeAsync(ActivationCode activationCode)
        {
            await _context.ActivationCodes.AddAsync(activationCode);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteActivationCodeAsync(ActivationCode activationCode)
        {
            _context.ActivationCodes.Remove(activationCode);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteActivationCodeAsync(string email)
        {
            var activationCode = await _context.ActivationCodes.FirstOrDefaultAsync(ac => ac.Email == email);
            if (activationCode != null)
            {
                _context.ActivationCodes.Remove(activationCode);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<string> HasActiveValidationCode(string email)
        {
            var activationCode = await _context.ActivationCodes.FirstOrDefaultAsync(ac => ac.Email == email && ac.Expiration >= DateTime.UtcNow);
            return activationCode?.Code;
        }
    }
}
