using ReadNest.Contracts.Users.Requests;
using ReadNest.Contracts.Users.Responses;

namespace ReadNest.Domain.Users.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> RegisterUserAsync(UserCreationDto userDto);

        Task<AuthResponseDto?> LoginUserAsync(UserLoginDto userDto);

        Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken);

        Task LogoutUserAsync(int userId);
    }
}
