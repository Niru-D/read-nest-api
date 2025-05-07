using ReadNest.Contracts.Users.Requests;
using ReadNest.Contracts.Users.Responses;
using ReadNest.Domain.Users.Entities;

namespace ReadNest.Tests.Users.ServiceTestData
{
    /// <summary>
    /// Encapsulates all necessary test data for User Service tests.
    /// </summary>
    public class UserServiceTestDataModel
    {
        public User UserEntity { get; set; }

        public UserDto UserDto { get; set; }

        public UserUpdateDto UserUpdateDto { get; set; }

        public UserDto UpdatedUserDto { get; set; }

        public UserCreationDto UserCreationDto { get; set; }

        public UserLoginDto UserLoginDto { get; set; }

        /// <summary>
        /// Constructor to initialize all test data objects.
        /// </summary>
        public UserServiceTestDataModel(User userEntity, UserDto userDto, UserUpdateDto userUpdateDto,
                                 UserDto updatedUserDto, UserCreationDto userCreationDto, UserLoginDto userLoginDto)
        {
            UserEntity = userEntity;
            UserDto = userDto;
            UserUpdateDto = userUpdateDto;
            UpdatedUserDto = updatedUserDto;
            UserCreationDto = userCreationDto;
            UserLoginDto = userLoginDto;
        }
    }
}
