using ReadNest.Domain.Users.Entities;

namespace ReadNest.Domain.Users.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetRefreshTokenAsync(string token);

        Task SaveRefreshTokenAsync(RefreshToken refreshToken);

        Task UpdateRefreshTokenAsync(RefreshToken refreshToken);

        Task RevokeAllExistingValidUserRefreshTokensAsync(int userId);
    }
}
