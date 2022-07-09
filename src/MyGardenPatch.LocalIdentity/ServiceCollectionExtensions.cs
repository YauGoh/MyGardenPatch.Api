using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyGardenPatch.Users.Services;

namespace MyGardenPatch.LocalIdentity;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLocalIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<ILocalIdentityManager, LocalIdentityMananger>();

        services
            .AddDbContext<LocalIdentityDbContext>(
                options => options.UseSqlServer(configuration.GetConnectionString("LocalIdentity")));

        services
            .AddIdentityCore<LocalIdentityUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 1;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddSignInManager<SignInManager<LocalIdentityUser>>()
            .AddEntityFrameworkStores<LocalIdentityDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }
}