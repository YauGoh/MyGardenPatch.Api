using Microsoft.Identity.Web;
using MyGardenPatch.Common;
using MyGardenPatch.LocalIdentity;
using MyGardenPatch.SqlServer;
using MyGardenPatch.Webapi.Services;

namespace MyGardenPatch.Webapi;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMyGardenPatchWebApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMicrosoftIdentityWebApiAuthentication(configuration);
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserProvider, HttpCurrentUserProvider>();

        services.AddMyGardenPatch(configuration);
        services.AddMyGardenBedSqlServer(configuration);

        services.AddLocalIdentity(configuration);

        return services;
    }
}
