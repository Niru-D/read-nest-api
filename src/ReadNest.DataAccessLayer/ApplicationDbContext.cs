using Microsoft.EntityFrameworkCore;
using ReadNest.Domain.BookLoans.Entities;
using ReadNest.Domain.Books.Entities;
using ReadNest.Domain.Users.Entities;

namespace ReadNest.DataAccessLayer
{
    public class ApplicationDbContext : DbContext
    {
        private readonly EntityTimestampInterceptor _entityTimestampInterceptor;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, EntityTimestampInterceptor entityTimestampInterceptor) 
            : base(options) {
            _entityTimestampInterceptor = entityTimestampInterceptor;
        }

        public DbSet<Book> Books { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<BookLoan> BookLoans { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(_entityTimestampInterceptor);
        }

    }
}
