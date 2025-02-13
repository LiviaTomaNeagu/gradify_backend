using Microsoft.EntityFrameworkCore;
using MyBackedApi.Data;
using MyBackedApi.Models;
using Infrastructure.Exceptions;

namespace MyBackendApi.Repositories
{
    public class UserAuthTokenRepository : BaseRepository
    {
        public UserAuthTokenRepository(ApplicationDbContext context) : base(context) { }

        public async Task<UserAuthToken> GetByRefreshTokenAsync(string refreshToken)
        {
            var result = _context.UserAuthTokens
                .Where(t => t.RefreshToken == refreshToken)

                .FirstOrDefaultAsync();

            if (result == null)
                throw new ResourceMissingException("The desired token does not exist");

            return await result;
        }

        public async Task AddRefreshTokenAsync(UserAuthToken userAuthToken)
        {
            _context.UserAuthTokens.Add(userAuthToken);

            await _context.SaveChangesAsync();
        }
    }
}
