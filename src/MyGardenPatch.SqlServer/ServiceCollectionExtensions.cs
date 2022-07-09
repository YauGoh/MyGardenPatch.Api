using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyGardenPatch.Aggregates;

namespace MyGardenPatch.SqlServer;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMyGardenBedSqlServer(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddDbContext<MyGardenPatchDbContext>(
            options => options.UseSqlServer(
                configuration.GetConnectionString("MyGardenPatch")));

        RegisterRepositories(services);

        return services;
    }

    private static void RegisterRepositories(IServiceCollection services)
    {
        services.AddScoped(
            typeof(IRepository<,>), 
            typeof(EntityFrameworkRepository<,>));
    }
}
