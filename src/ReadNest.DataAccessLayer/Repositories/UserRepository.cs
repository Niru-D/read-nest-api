using Microsoft.EntityFrameworkCore;
using ReadNest.Contracts.Users;
using ReadNest.Domain.Users.Entities;
using ReadNest.Domain.Users.Interfaces;

namespace ReadNest.DataAccessLayer.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<User>, int)> GetUsersAsync(string? firstName, string? lastName, string? email, Role? role, string? searchQuery, int pageNumber, int pageSize)
        {
            var collection = _context.Users as IQueryable<User>;
            if (!string.IsNullOrWhiteSpace(firstName))
            {
                firstName = firstName.Trim();
                collection = collection.Where(u => u.FirstName == firstName);
            }
            if (!string.IsNullOrWhiteSpace(lastName))
            {
                lastName = lastName.Trim();
                collection = collection.Where(u => u.LastName == lastName);
            }
            if (!string.IsNullOrWhiteSpace(email))
            {
                email = email.Trim();
                collection = collection.Where(u => u.Email == email);
            }
            if (role.HasValue)
            {
                collection = collection.Where(u => u.Role == role);
            }
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(u =>
                    u.FirstName.Contains(searchQuery) ||
                    u.LastName.Contains(searchQuery) ||
                    u.Email.Contains(searchQuery) ||
                    (u.Address != null && u.Address.Contains(searchQuery)) ||
                    (u.ContactNumber != null && u.ContactNumber.Contains(searchQuery))
                );
            }

            var totalItemCount = await collection.CountAsync();
            var usersCollection = await collection.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();

            return (usersCollection, totalItemCount);
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> UserExistsAsync(int id)
        {
            return await _context.Users.AnyAsync(u => u.Id == id);
        }

        public async Task CreateUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
        }

        public void DeleteUser(User user)
        {
            _context.Users.Remove(user);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
        
    }
}
