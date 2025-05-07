using Microsoft.EntityFrameworkCore;
using ReadNest.Domain.Users.Entities;
using ReadNest.Domain.Users.Interfaces;

namespace ReadNest.DataAccessLayer.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _context;

        public RefreshTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            return await _context.RefreshTokens.SingleOrDefaultAsync(r => r.Token == token);
        }

        public async Task SaveRefreshTokenAsync(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRefreshTokenAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task RevokeAllExistingValidUserRefreshTokensAsync(int userId)
        {
            var tokens = await _context.RefreshTokens
                            .Where(rt => (rt.UserId == userId) && (rt.IsRevoked == false) && (rt.IsUsed == false) && (rt.ExpiryDate > DateTime.Now))
                            .ToListAsync();
            if (tokens.Count != 0)
            {
                foreach (var token in tokens)
                {
                    token.IsRevoked = true;
                }
                await _context.SaveChangesAsync();
            }
        }
    }
}
