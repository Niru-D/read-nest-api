using ReadNest.Contracts.Users.Requests;
using ReadNest.Contracts.Users.Responses;
using ReadNest.Domain.Users.Interfaces;
using Microsoft.Extensions.Configuration;
using ReadNest.Domain.Users.Entities;
using AutoMapper;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace ReadNest.Services.Users
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AuthService(IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository, IConfiguration configuration, IMapper mapper)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<AuthResponseDto?> RegisterUserAsync(UserCreationDto userDto)
        {
            var (existingUser, count) = await _userRepository.GetUsersAsync(null, null, userDto.Email, null, null, 1, 1);
            if (existingUser.Any()) 
            {
                return null;
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
            var userEntity = _mapper.Map<User>(userDto);
            userEntity.Password = hashedPassword;

            await _userRepository.CreateUserAsync(userEntity);
            await _userRepository.SaveChangesAsync();

            var token = GenerateJwtToken(userEntity);
            return new AuthResponseDto { Token = token, Id = userEntity.Id, Email = userEntity.Email };
        }

        public async Task<AuthResponseDto?> LoginUserAsync(UserLoginDto userDto)
        {
            var (user, count) = await _userRepository.GetUsersAsync(null, null, userDto.Email, null, null, 1, 1);
            if (user == null || !BCrypt.Net.BCrypt.Verify(userDto.Password, user.First().Password))
            {
                return null;
            }

            await _refreshTokenRepository.RevokeAllExistingValidUserRefreshTokensAsync(user.First().Id);

            var token = GenerateJwtToken(user.First());
            var refreshToken = GenerateRefreshToken(user.First().Id);

            await _refreshTokenRepository.SaveRefreshTokenAsync(refreshToken);

            return new AuthResponseDto 
            { 
                Token = token, 
                RefreshToken = refreshToken.Token, 
                Id = user.First().Id, 
                Email = user.First().Email 
            };
        }

        public async Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken)
        {
            var storedRefreshToken = await _refreshTokenRepository.GetRefreshTokenAsync(refreshToken);

            if (storedRefreshToken == null || storedRefreshToken.IsRevoked || storedRefreshToken.IsUsed || storedRefreshToken.ExpiryDate < DateTime.Now)
            {
                return null;
            }

            storedRefreshToken.IsUsed = true;
            await _refreshTokenRepository.UpdateRefreshTokenAsync(storedRefreshToken);

            var user = await _userRepository.GetUserByIdAsync(storedRefreshToken.UserId);
            if (user != null)
            {
                var newAccessToken = GenerateJwtToken(user);

                var newRefreshToken = GenerateRefreshToken(user.Id);
                await _refreshTokenRepository.SaveRefreshTokenAsync(newRefreshToken);

                return new AuthResponseDto
                {
                    Token = newAccessToken,
                    RefreshToken = newRefreshToken.Token,
                    Id = user.Id,
                    Email = user.Email
                };
            }
            return null;
        }

        public async Task LogoutUserAsync(int userId)
        {
            await _refreshTokenRepository.RevokeAllExistingValidUserRefreshTokensAsync(userId);
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]));

            var claims = new List<Claim>
            {
                new Claim("sub", user.Id.ToString()),
                new Claim("given_name", user.FirstName),
                new Claim("family_name", user.LastName),
                new Claim("email", user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiryMinutes"])),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private RefreshToken GenerateRefreshToken(int userId)
        {
            var refreshToken = new RefreshToken
            {
                UserId = userId,
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)),
                ExpiryDate = DateTime.Now.AddDays(7),
                IsRevoked = false,
                IsUsed = false
            };

            return refreshToken;
        }
    }
}