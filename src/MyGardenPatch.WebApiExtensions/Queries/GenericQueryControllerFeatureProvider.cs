namespace MyGardenPatch.WebApiExtensions.Queries;

internal class GenericQueryControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
{
    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
    {
        var controllers = GetControllerTypes();

        foreach (var controller in controllers)
            feature.Controllers.Add(controller);
    }

    private IEnumerable<TypeInfo> GetControllerTypes()
    {
        var genericControllerType = typeof(GenericQueryController<,>);

        var queries = Assembly
            .GetAssembly(typeof(IQuery<>))!
            .GetTypes()
            .Where(t => IsQuery(t))
            .Select(t => new { Query = t, Result = GetResultType(t) })
            .Select(a => genericControllerType.MakeGenericType(a.Query, a.Result).GetTypeInfo())
            .ToList();

        return queries;
    }

    private Type GetResultType(Type t)
    {
        return t
            .FindInterfaces((i, _) => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQuery<>), null)
            .Select(t => t.GetGenericArguments().Single())
            .Single();
    }

    private bool IsQuery(Type type)
    {
        return !type.IsAbstract && !type.IsInterface &&
            type.FindInterfaces((i, _) => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQuery<>), null).Any();
    }
}