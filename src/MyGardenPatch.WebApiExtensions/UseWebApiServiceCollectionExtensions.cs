using MyGardenPatch.Users.Commands;

namespace MyGardenPatch.WebApiExtensions;
public static class UseWebapiServiceCollectionExtensions
{
    public static TMvcBuilder AddCommandControllers<TMvcBuilder>(
        this TMvcBuilder builder) where TMvcBuilder : IMvcBuilder
    {
        builder.ConfigureApplicationPartManager(
            manager =>
            {
                manager.FeatureProviders.Add(
                    new GenericCommandControllerFeatureProvider());
                manager.FeatureProviders.Add(
                    new GenericQueryControllerFeatureProvider());
            });

        return builder;
    }

    //public static void AddCommands(this WebApplication app)
    //{
    //    app.MapPost("/commands/test", CommandDelegateFactory.GetDelegate<RegisterUserCommand>());
    //}
}