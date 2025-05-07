using ReadNest.Contracts.Users.Requests;
using ReadNest.Contracts.Users.Responses;
using ReadNest.Domain.Users.Entities;
using System.Collections;

namespace ReadNest.Tests.Users.ServiceTestData
{
    class UserServiceTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var userEntity = new User
            {
                Id = 1,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@example.com",
                Address = "789 Pine Road, Chicago, IL",
                ContactNumber = "+1-555-345-6789",
                Role = Contracts.Users.Role.LibraryMember
            };
            var userDto = new UserDto
            {
                Id = 1,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@example.com",
                Address = "789 Pine Road, Chicago, IL",
                ContactNumber = "+1-555-345-6789",
                Role = Contracts.Users.Role.LibraryMember
            };
            var userUpdateDto = new UserUpdateDto
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@example.com",
                Address = "789 Pine Road, Chicago, IL",
                ContactNumber = "+1-555-345-6789",
                Role = Contracts.Users.Role.LibraryMember
            };
            var updatedUserDto = new UserDto
            {
                Id = 1,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                FirstName = "James",
                LastName = "Donalds",
                Email = "jamese@example.com",
                Address = "789 Pine Road, Chicago, IL",
                ContactNumber = "+1-555-345-60000",
                Role = Contracts.Users.Role.LibraryMember
            };
            var userCreationDto = new UserCreationDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@example.com",
                Address = "789 Pine Road, Chicago, IL",
                ContactNumber = "+1-555-345-6789",
                Role = Contracts.Users.Role.LibraryMember,
                Password = "asdf123"
            };
            var userLoginDto = new UserLoginDto
            {
                Email = "johndoe@example.com",
                Password = "asdf123"
            };
            yield return new object[] { new UserServiceTestDataModel(userEntity, userDto, userUpdateDto, updatedUserDto, userCreationDto, userLoginDto) };
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
