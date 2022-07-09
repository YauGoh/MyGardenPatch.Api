using Microsoft.Extensions.DependencyInjection;
using MyGardenPatch.WebApiExtensions.Commands;

namespace MyGardenPatch.WebApiExtensions;
public static class UseWebapiServiceCollectionExtensions
{
    public static TMvcBuilder AddCommandControllers<TMvcBuilder>(
        this TMvcBuilder builder) where TMvcBuilder : IMvcBuilder
    {
        builder.ConfigureApplicationPartManager(
            manager => manager.FeatureProviders.Add(
                new GenericCommandControllerFeatureProvider()));

        return builder;
    }
}