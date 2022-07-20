namespace MyGardenPatch.WebApiExtensions;
public static class UseWebapiServiceCollectionExtensions
{
    public static IServiceCollection AddRoleBasedAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(configure =>
        {
            configure.AddPolicy(WellKnownRoles.Gardener, policy => policy.RequireRole(WellKnownRoles.Gardener));
            configure.AddPolicy(WellKnownRoles.Api, policy => policy.RequireRole(WellKnownRoles.Api));
        });

        return services;
    }

    public static void AddQueries(this IEndpointRouteBuilder app)
    {
        foreach (var query in QueryDelegateFactory.ResolveQueryRoutes())
        {
            app
                .MapPost(
                    query.Route,
                    query.Delegate)
                .RequireAuthorization(query.Roles);
        }
    }

    public static void AddCommands(this IEndpointRouteBuilder app)
    {
        foreach(var command in CommandDelegateFactory.ResolveCommandRoutes())
        {
            app
                .MapPost(
                    command.Route, 
                    command.Delegate)
                .RequireAuthorization(command.Roles);
        }
        
    }
}