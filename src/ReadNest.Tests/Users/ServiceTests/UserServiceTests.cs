using AutoMapper;
using Moq;
using ReadNest.Contracts.Users.Responses;
using ReadNest.Contracts;
using ReadNest.Domain.Users.Entities;
using ReadNest.Domain.Users.Interfaces;
using ReadNest.Services.Users;
using ReadNest.Tests.Users.ServiceTestData;

namespace ReadNest.Tests.Users.ServiceTests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockRepository = new Mock<IUserRepository>();
            _mockMapper = new Mock<IMapper>();
            _userService = new UserService(_mockRepository.Object, _mockMapper.Object);
        }

        [Theory]
        [ClassData(typeof(UserServiceTestData))]
        public async Task GetUsersAsync_GetAllUsers_ShouldReturnUsersAndMetaData(UserServiceTestDataModel testData)
        {
            // Arrange
            var users = new List<User> { testData.UserEntity };
            var userDtos = new List<UserDto> { testData.UserDto };
            var metadata = new PaginationMetaData(1, 10, 1);

            _mockRepository.Setup(repo => repo.GetUsersAsync(null, null, null, null, null, 1, 10))
                            .ReturnsAsync((users, 1));
            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<UserDto>>(users)).Returns(userDtos);

            // Act
            var result = await _userService.GetUsersAsync(null, null, null, null, null, 1, 10);

            // Assert
            Assert.Equal(userDtos, result.Item1);
            Assert.Equal(metadata.TotalItemCount, result.Item2.TotalItemCount);
        }

        [Theory]
        [ClassData(typeof(UserServiceTestData))]
        public async Task GetUserByIdAsync_GetUserById_ShouldReturnUser_WhenUserExists(UserServiceTestDataModel testData)
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetUserByIdAsync(1)).ReturnsAsync(testData.UserEntity);
            _mockMapper.Setup(mapper => mapper.Map<UserDto>(testData.UserEntity)).Returns(testData.UserDto);

            // Act
            var result = await _userService.GetUserByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testData.UserDto, result);
        }

        [Theory]
        [ClassData(typeof(UserServiceTestData))]
        public async Task UpdateUserAsync_UpdateUser_ShouldReturnUpdatedUser_WhenUserExists(UserServiceTestDataModel testData)
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetUserByIdAsync(testData.UserUpdateDto.Id)).ReturnsAsync(testData.UserEntity);
            _mockRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);
            _mockMapper.Setup(mapper => mapper.Map<UserDto>(testData.UserEntity)).Returns(testData.UpdatedUserDto);

            // Act
            var result = await _userService.UpdateUserAsync(testData.UserUpdateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testData.UpdatedUserDto.FirstName, result.FirstName);
        }

        [Theory]
        [ClassData(typeof(UserServiceTestData))]
        public async Task UpdateUserAsync_UpdateUser_ShouldReturnNull_WhenUserDoesNotExist(UserServiceTestDataModel testData)
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetUserByIdAsync(testData.UserUpdateDto.Id)).ReturnsAsync((User)null);

            // Act
            var result = await _userService.UpdateUserAsync(testData.UserUpdateDto);

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [ClassData(typeof(UserServiceTestData))]
        public async Task DeleteUserAsync_DeleteUser_ShouldReturnTrue_WhenUserExists(UserServiceTestDataModel testData)
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetUserByIdAsync(testData.UserEntity.Id)).ReturnsAsync(testData.UserEntity);
            _mockRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

            // Act
            var result = await _userService.DeleteUserAsync(testData.UserEntity.Id);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteUserAsync_DeleteUser_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetUserByIdAsync(It.IsAny<int>())).ReturnsAsync((User)null);

            // Act
            var result = await _userService.DeleteUserAsync(1);

            // Assert
            Assert.False(result);
        }
    }
}
