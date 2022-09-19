using System.Text.Json;

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
        => async (HttpRequest request, ICommandExecutor commandExecutor, IFileAttachments fileAttachments, CancellationToken cancellationToken) =>
        {
            return await ExceptionHandler.Try<TCommand>(async () =>
            {
                if (request.HasFormContentType)
                {
                    await ProcessFromFormContent<TCommand>(request, commandExecutor, fileAttachments, cancellationToken);
                }

                if (request.HasJsonContentType())
                {
                    await ProcessFromJsonContent<TCommand>(request, commandExecutor, cancellationToken);
                }
            });
        };

    private static async Task ProcessFromFormContent<TCommand>(HttpRequest request, ICommandExecutor commandExecutor, IFileAttachments fileAttachments, CancellationToken cancellationToken) where TCommand : ICommand
    {
        foreach (var file in request.Form.Files.Where(f => f.ContentType != "application/json"))
        {
            var filename = file.FileName;
            var contentType = file.ContentType;
            var stream = file.OpenReadStream();

            fileAttachments.Add(
                new FileAttachment(
                    new Guid(file.Headers["gardenId"]),
                    new Guid(file.Headers["imageId"]),
                    filename,
                    contentType,
                    stream));
        }

        var commandFile = request.Form.Files.FirstOrDefault(f => f.ContentType == "application/json");
        if (commandFile is not null)
        {
            using var stream = commandFile.OpenReadStream();

            var command = await JsonSerializer.DeserializeAsync<TCommand>(stream, cancellationToken: cancellationToken);

            await commandExecutor.HandleAsync(command!, cancellationToken);
        }
    }

    private static async Task ProcessFromJsonContent<TCommand>(HttpRequest request, ICommandExecutor commandExecutor, CancellationToken cancellationToken) where TCommand : ICommand
    {
        var command = await request.ReadFromJsonAsync<TCommand>(cancellationToken);

        await commandExecutor.HandleAsync(command!, cancellationToken);
    }

    private static string[] GetRoles(Type command) 
        => command
            .GetCustomAttributes<RoleAttribute>()
            .Select(a => a.Role)
            .ToArray();
    
}
