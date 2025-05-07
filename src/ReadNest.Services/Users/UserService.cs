using AutoMapper;
using ReadNest.Contracts;
using ReadNest.Contracts.Users;
using ReadNest.Contracts.Users.Requests;
using ReadNest.Contracts.Users.Responses;
using ReadNest.Domain.Users.Entities;
using ReadNest.Domain.Users.Interfaces;

namespace ReadNest.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<(IEnumerable<UserDto>, PaginationMetaData)> GetUsersAsync(string? firstName, string? lastName, string? email, Role? role, string? searchQuery, int pageNumber, int pageSize)
        {
            var (users, totalItemCount) = await _repository.GetUsersAsync(firstName, lastName, email, role, searchQuery, pageNumber, pageSize);
            var paginationMetadata = new PaginationMetaData(totalItemCount, pageSize, pageNumber);

            return (_mapper.Map<IEnumerable<UserDto>>(users), paginationMetadata);
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _repository.GetUserByIdAsync(id);
            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> UserExistsAsync(int id)
        {
            return await _repository.UserExistsAsync(id);
        }

        public async Task<UserDto?> UpdateUserAsync(UserUpdateDto userToUpdate)
        {
            var existingUser = await _repository.GetUserByIdAsync(userToUpdate.Id);

            if (existingUser == null)
            {
                return null;
            }

            existingUser.FirstName = userToUpdate.FirstName;
            existingUser.LastName = userToUpdate.LastName;
            existingUser.Email = userToUpdate.Email;
            existingUser.Address = userToUpdate.Address;
            existingUser.ContactNumber = userToUpdate.ContactNumber;
            existingUser.Password = BCrypt.Net.BCrypt.HashPassword(userToUpdate.Password);

            _repository.UpdateUser(existingUser);
            await _repository.SaveChangesAsync();

            return _mapper.Map<UserDto>(existingUser);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _repository.GetUserByIdAsync(id);

            if (user == null)
            {
                return false;
            }
            _repository.DeleteUser(user);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
