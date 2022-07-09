using Microsoft.AspNetCore.Mvc.ApplicationParts;
using MyGardenPatch.Commands;
using System.Reflection;

namespace MyGardenPatch.WebApiExtensions.Commands;

internal class GenericCommandControllerApplicationPart : ApplicationPart, IApplicationPartTypeProvider
{
    private IEnumerable<TypeInfo>? _types;

    public IEnumerable<TypeInfo> Types => _types ?? (_types = InitializeTypes());

    public override string Name => typeof(GenericCommandController<>).Name;

    private IEnumerable<TypeInfo> InitializeTypes()
    {
        var genericControllerType = typeof(GenericCommandController<>);

        var commands = Assembly
            .GetAssembly(typeof(ICommand))!
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(ICommand)) && !t.IsAbstract && !t.IsInterface)
            .Select(t => genericControllerType.MakeGenericType(t).GetTypeInfo())
            .ToList();

        return commands;
    }
}