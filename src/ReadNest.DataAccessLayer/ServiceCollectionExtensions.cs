using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReadNest.DataAccessLayer.Repositories;
using ReadNest.Domain.BookLoans.Interfaces;
using ReadNest.Domain.Books.Interfaces;
using ReadNest.Domain.Users.Interfaces;

namespace ReadNest.DataAccessLayer
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPersistenceServices (this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<EntityTimestampInterceptor>();
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("ReadNestDBConnectionString"));
            });

            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBookLoanRepository, BookLoanRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            return services;
        }
    }
}
