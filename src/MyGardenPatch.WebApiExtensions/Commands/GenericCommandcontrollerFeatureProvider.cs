namespace MyGardenPatch.WebApiExtensions.Commands;

internal class GenericCommandControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
{
    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
    {
        var genericControllerType = typeof(GenericCommandController<>);

        var controllers = Assembly
            .GetAssembly(typeof(ICommand))!
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(ICommand)) && !t.IsAbstract && !t.IsInterface)
            .Select(t => genericControllerType.MakeGenericType(t).GetTypeInfo())
            .ToList();

        foreach (var controller in controllers)
            feature.Controllers.Add(controller);
    }
}