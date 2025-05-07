using Microsoft.Extensions.DependencyInjection;
using ReadNest.Domain.Books.Interfaces;
using ReadNest.Domain.Users.Interfaces;
using ReadNest.Services.Users;
using ReadNest.Services.Books;
using ReadNest.Domain.BookLoans.Interfaces;
using ReadNest.Services.BookLoans;

namespace ReadNest.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices (this IServiceCollection services)
        {
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IBookLoanService, BookLoanService>();
            services.AddScoped<IAuthService, AuthService>();
            return services;
        }
    }
}
