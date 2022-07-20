namespace MyGardenPatch.WebApiExtensions;

internal record CommandRoute(string Route, Delegate Delegate, string[] Roles);

internal static class CommandDelegateFactory
{
    internal static IEnumerable<CommandRoute> ResolveCommandRoutes()
    {
        var methodInfo = typeof(CommandDelegateFactory).GetMethod(nameof(GetDelegate), BindingFlags.Static | BindingFlags.NonPublic);

        var commands = Assembly
           .GetAssembly(typeof(ICommand))!
           .GetTypes()
           .Where(t => t.IsAssignableTo(typeof(ICommand)) && !t.IsAbstract && !t.IsInterface)
           .Select(t => new CommandRoute(
               $"/commands/{t.Name}", 
               (Delegate)methodInfo!.MakeGenericMethod(t).Invoke(null, null)!, 
               GetRoles(t)))
           .ToList();

        return commands;
    }

    internal static Delegate GetDelegate<TCommand>() where TCommand : ICommand
        => async (HttpRequest request, ICommandExecutor commandExecutor, CancellationToken cancellationToken) =>
        {
            var command = await request.ReadFromJsonAsync<TCommand>(cancellationToken);

            await commandExecutor.HandleAsync(command!, cancellationToken);
        };

    private static string[] GetRoles(Type command) 
        => command
            .GetCustomAttributes<RoleAttribute>()
            .Select(a => a.Role)
            .ToArray();
    
}
