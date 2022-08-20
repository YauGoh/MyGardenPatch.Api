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

    public static void AddCommandsAndQueries(this WebApplication webApplication, string corsPolicyName = "WebApp")
    {
        webApplication.AddQueries(corsPolicyName);
        webApplication.AddCommands(corsPolicyName);
    }

    public static void AddQueries(this IEndpointRouteBuilder app, string corsPolicyName)
    {
        foreach (var query in QueryDelegateFactory.ResolveQueryRoutes())
        {
            app
                .MapPost(
                    query.Route,
                    query.Delegate)
                .RequireAuthorization(query.Roles)
                .RequireCors(corsPolicyName);
        }
    }

    public static void AddCommands(this IEndpointRouteBuilder app, string corsPolicyName)
    {
        foreach(var command in CommandDelegateFactory.ResolveCommandRoutes())
        {
            app
                .MapPost(
                    command.Route, 
                    command.Delegate)
                .RequireAuthorization(command.Roles)
                .RequireCors(corsPolicyName);
        }
        
    }
}