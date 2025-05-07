using AutoMapper;
using Microsoft.Extensions.Configuration;
using Moq;
using ReadNest.Domain.Users.Entities;
using ReadNest.Domain.Users.Interfaces;
using ReadNest.Services.Users;
using ReadNest.Tests.Users.ServiceTestData;

namespace ReadNest.Tests.Users.ServiceTests
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IRefreshTokenRepository> _mockRefreshTokenRepository;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IMapper> _mockMapper;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockMapper = new Mock<IMapper>();
            _authService = new AuthService(_mockUserRepository.Object, _mockRefreshTokenRepository.Object, _mockConfiguration.Object, _mockMapper.Object);
        }

        [Theory]
        [ClassData(typeof(UserServiceTestData))]
        public async Task RegisterUserAsync_CreateUser_ShouldReturnAuthResponse_WhenUserDoesNotExist(UserServiceTestDataModel testData)
        {
            // Arrange
            _mockUserRepository.Setup(repo => repo.GetUsersAsync(null, null, testData.UserCreationDto.Email, null, null, 1, 1))
                               .ReturnsAsync((new List<User>(), 0));
            _mockMapper.Setup(mapper => mapper.Map<User>(testData.UserCreationDto)).Returns(testData.UserEntity);

            var jwtSettingsSection = new Mock<IConfigurationSection>();
            jwtSettingsSection.Setup(x => x["Secret"]).Returns("supersecuresecretkeywithmorethan32chars");
            jwtSettingsSection.Setup(x => x["Issuer"]).Returns("issuer");
            jwtSettingsSection.Setup(x => x["Audience"]).Returns("audience");
            jwtSettingsSection.Setup(x => x["ExpiryMinutes"]).Returns("30");

            _mockConfiguration.Setup(config => config.GetSection("JwtSettings"))
                              .Returns(jwtSettingsSection.Object);

            // Act
            var result = await _authService.RegisterUserAsync(testData.UserCreationDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testData.UserEntity.Email, result.Email);
            Assert.Equal(testData.UserEntity.Id, result.Id);
            Assert.NotNull(result.Token);
        }

        [Theory]
        [ClassData(typeof(UserServiceTestData))]
        public async Task RegisterUserAsync_CreateUser_ShouldReturnNull_WhenUserAlreadyExists(UserServiceTestDataModel testData)
        {
            // Arrange
            _mockUserRepository.Setup(repo => repo.GetUsersAsync(null, null, testData.UserCreationDto.Email, null, null, 1, 1))
                               .ReturnsAsync((new List<User> { testData.UserEntity }, 1));

            // Act
            var result = await _authService.RegisterUserAsync(testData.UserCreationDto);

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [ClassData(typeof(UserServiceTestData))]
        public async Task LoginUserAsync_LoginUser_ShouldReturnAuthResponse_WhenCredentialsAreValid(UserServiceTestDataModel testData)
        {
            // Arrange
            testData.UserEntity.Password = BCrypt.Net.BCrypt.HashPassword(testData.UserLoginDto.Password);

            _mockUserRepository.Setup(repo => repo.GetUsersAsync(null, null, testData.UserLoginDto.Email, null, null, 1, 1))
                               .ReturnsAsync((new List<User> { testData.UserEntity }, 1));

            _mockRefreshTokenRepository.Setup(repo => repo.RevokeAllExistingValidUserRefreshTokensAsync(testData.UserEntity.Id))
                                       .Returns(Task.CompletedTask);

            _mockRefreshTokenRepository.Setup(repo => repo.SaveRefreshTokenAsync(It.IsAny<RefreshToken>()))
                                       .Returns(Task.CompletedTask);

            var jwtSettingsSection = new Mock<IConfigurationSection>();
            jwtSettingsSection.Setup(x => x["Secret"]).Returns("supersecuresecretkeywithmorethan32chars");
            jwtSettingsSection.Setup(x => x["Issuer"]).Returns("issuer");
            jwtSettingsSection.Setup(x => x["Audience"]).Returns("audience");
            jwtSettingsSection.Setup(x => x["ExpiryMinutes"]).Returns("30");

            _mockConfiguration.Setup(config => config.GetSection("JwtSettings"))
                              .Returns(jwtSettingsSection.Object);

            // Act
            var result = await _authService.LoginUserAsync(testData.UserLoginDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testData.UserEntity.Email, result.Email);
            Assert.Equal(testData.UserEntity.Id, result.Id);
            Assert.NotNull(result.Token);
            Assert.NotNull(result.RefreshToken);

            _mockRefreshTokenRepository.Verify(repo => repo.RevokeAllExistingValidUserRefreshTokensAsync(testData.UserEntity.Id), Times.Once);
            _mockRefreshTokenRepository.Verify(repo => repo.SaveRefreshTokenAsync(It.IsAny<RefreshToken>()), Times.Once);
        }

        [Theory]
        [ClassData(typeof(UserServiceTestData))]
        public async Task LoginUserAsync_LoginUser_ShouldReturnNull_WhenCredentialsAreInvalid(UserServiceTestDataModel testData)
        {
            // Arrange
            testData.UserEntity.Password = BCrypt.Net.BCrypt.HashPassword("asdf123");
            testData.UserLoginDto.Password = "WrongPassword";
            _mockUserRepository.Setup(repo => repo.GetUsersAsync(null, null, testData.UserLoginDto.Email, null, null, 1, 1))
                               .ReturnsAsync((new List<User> { testData.UserEntity }, 1));
            var jwtSettingsSection = new Mock<IConfigurationSection>();
            jwtSettingsSection.Setup(x => x["Secret"]).Returns("supersecuresecretkeywithmorethan32chars");
            jwtSettingsSection.Setup(x => x["Issuer"]).Returns("issuer");
            jwtSettingsSection.Setup(x => x["Audience"]).Returns("audience");
            jwtSettingsSection.Setup(x => x["ExpiryMinutes"]).Returns("30");

            _mockConfiguration.Setup(config => config.GetSection("JwtSettings"))
                              .Returns(jwtSettingsSection.Object);

            // Act
            var result = await _authService.LoginUserAsync(testData.UserLoginDto);

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [ClassData(typeof(UserServiceTestData))]
        public async Task RefreshTokenAsync_GetNewAccessToken_ShouldReturnNewTokens_WhenRefreshTokenIsValid(UserServiceTestDataModel testData)
        {
            // Arrange
            var validRefreshToken = new RefreshToken
            {
                Token = "valid-refresh-token",
                UserId = testData.UserEntity.Id,
                ExpiryDate = DateTime.Now.AddDays(1),
                IsRevoked = false,
                IsUsed = false
            };

            _mockRefreshTokenRepository.Setup(repo => repo.GetRefreshTokenAsync(validRefreshToken.Token))
                                       .ReturnsAsync(validRefreshToken);

            _mockUserRepository.Setup(repo => repo.GetUserByIdAsync(testData.UserEntity.Id))
                               .ReturnsAsync(testData.UserEntity);

            _mockRefreshTokenRepository.Setup(repo => repo.SaveRefreshTokenAsync(It.IsAny<RefreshToken>()))
                                       .Returns(Task.CompletedTask);

            _mockRefreshTokenRepository.Setup(repo => repo.UpdateRefreshTokenAsync(validRefreshToken))
                                       .Returns(Task.CompletedTask);

            var jwtSettingsSection = new Mock<IConfigurationSection>();
            jwtSettingsSection.Setup(x => x["Secret"]).Returns("supersecuresecretkeywithmorethan32chars");
            jwtSettingsSection.Setup(x => x["Issuer"]).Returns("issuer");
            jwtSettingsSection.Setup(x => x["Audience"]).Returns("audience");
            jwtSettingsSection.Setup(x => x["ExpiryMinutes"]).Returns("30");

            _mockConfiguration.Setup(config => config.GetSection("JwtSettings"))
                              .Returns(jwtSettingsSection.Object);

            // Act
            var result = await _authService.RefreshTokenAsync(validRefreshToken.Token);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testData.UserEntity.Email, result.Email);
            Assert.Equal(testData.UserEntity.Id, result.Id);
            Assert.NotNull(result.Token);
            Assert.NotNull(result.RefreshToken);

            _mockRefreshTokenRepository.Verify(repo => repo.UpdateRefreshTokenAsync(validRefreshToken), Times.Once);
            _mockRefreshTokenRepository.Verify(repo => repo.SaveRefreshTokenAsync(It.IsAny<RefreshToken>()), Times.Once);
        }

        [Fact]
        public async Task RefreshTokenAsync_GetNewAccessToken_ShouldReturnNull_WhenRefreshTokenIsInvalid()
        {
            // Arrange
            _mockRefreshTokenRepository.Setup(repo => repo.GetRefreshTokenAsync("invalid-token"))
                                       .ReturnsAsync((RefreshToken?)null);

            // Act
            var result = await _authService.RefreshTokenAsync("invalid-token");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task RefreshTokenAsync_GetNewAccessToken_ShouldReturnNull_WhenRefreshTokenIsExpired()
        {
            // Arrange
            var expiredRefreshToken = new RefreshToken
            {
                Token = "expired-refresh-token",
                UserId = 1,
                ExpiryDate = DateTime.Now.AddMinutes(-1),
                IsRevoked = false,
                IsUsed = false
            };

            _mockRefreshTokenRepository.Setup(repo => repo.GetRefreshTokenAsync(expiredRefreshToken.Token))
                                       .ReturnsAsync(expiredRefreshToken);

            // Act
            var result = await _authService.RefreshTokenAsync(expiredRefreshToken.Token);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task RefreshTokenAsync_GetNewAccessToken_ShouldReturnNull_WhenRefreshTokenIsRevoked()
        {
            // Arrange
            var revokedRefreshToken = new RefreshToken
            {
                Token = "revoked-refresh-token",
                UserId = 1,
                ExpiryDate = DateTime.Now.AddDays(1),
                IsRevoked = true,
                IsUsed = false
            };

            _mockRefreshTokenRepository.Setup(repo => repo.GetRefreshTokenAsync(revokedRefreshToken.Token))
                                       .ReturnsAsync(revokedRefreshToken);

            // Act
            var result = await _authService.RefreshTokenAsync(revokedRefreshToken.Token);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task RefreshTokenAsync_GetNewAccessToken_ShouldReturnNull_WhenRefreshTokenIsAlreadyUsed()
        {
            // Arrange
            var usedRefreshToken = new RefreshToken
            {
                Token = "used-refresh-token",
                UserId = 1,
                ExpiryDate = DateTime.Now.AddDays(1),
                IsRevoked = false,
                IsUsed = true
            };

            _mockRefreshTokenRepository.Setup(repo => repo.GetRefreshTokenAsync(usedRefreshToken.Token))
                                       .ReturnsAsync(usedRefreshToken);

            // Act
            var result = await _authService.RefreshTokenAsync(usedRefreshToken.Token);

            // Assert
            Assert.Null(result);
        }
    }
}
