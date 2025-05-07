using ReadNest.Contracts.Users;
using ReadNest.Domain.Users.Entities;

namespace ReadNest.Domain.Users.Interfaces
{
    public interface IUserRepository
    {
        Task<(IEnumerable<User>, int)> GetUsersAsync(
            string? firstName,
            string? lastName,
            string? email,
            Role? role,
            string? searchQuery,
            int pageNumber,
            int pageSize);

        Task<User?> GetUserByIdAsync(int id);

        Task<bool> UserExistsAsync(int id);

        Task CreateUserAsync(User user);

        void UpdateUser(User user);

        void DeleteUser(User user);

        Task<bool> SaveChangesAsync();
    }
}
