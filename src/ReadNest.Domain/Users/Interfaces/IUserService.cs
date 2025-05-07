using ReadNest.Contracts.Users.Requests;
using ReadNest.Contracts.Users.Responses;
using ReadNest.Contracts;
using ReadNest.Contracts.Users;

namespace ReadNest.Domain.Users.Interfaces
{
    public interface IUserService
    {
        Task<(IEnumerable<UserDto>, PaginationMetaData)> GetUsersAsync(
            string? firstName,
            string? lastName,
            string? email,
            Role? role,
            string? searchQuery,
            int pageNumber,
            int pageSize);

        Task<UserDto?> GetUserByIdAsync(int id);

        Task<bool> UserExistsAsync(int id);

        Task<UserDto?> UpdateUserAsync(UserUpdateDto userToUpdate);

        Task<bool> DeleteUserAsync(int id);
    }
}
