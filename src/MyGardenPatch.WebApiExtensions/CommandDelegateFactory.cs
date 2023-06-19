

using System.Text.Json.Serialization;
using System.Web;

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
        => async (HttpRequest request, ICommandExecutor commandExecutor, ICurrentUserProvider currentUser, IFileStorage fileStorage, CancellationToken cancellationToken) =>
        {
            return await ExceptionHandler.Try<TCommand>(async () =>
            {
                if (request.HasFormContentType)
                {
                    await ProcessFromFormContent<TCommand>(request, commandExecutor, currentUser, fileStorage, cancellationToken);
                }

                if (request.HasJsonContentType())
                {
                    await ProcessFromJsonContent<TCommand>(request, commandExecutor, cancellationToken);
                }
            });
        };

    private static async Task ProcessFromFormContent<TCommand>(HttpRequest request, ICommandExecutor commandExecutor, ICurrentUserProvider currentUser, IFileStorage fileStorage, CancellationToken cancellationToken) where TCommand : ICommand
    {
        if (currentUser.GardenerId is null) throw new UserNotAuthenticatedException();

        var gardenerId = currentUser.GardenerId.Value;

        var commandFile = request.Form.Files.FirstOrDefault(f => f.Name == "command" && f.ContentType == "application/json");
        if (commandFile is null) throw new ArgumentException("Command expected");
        
        using var stream = commandFile.OpenReadStream();
        var command = await JsonSerializer.DeserializeAsync<TCommand>(
            stream,

            // todo get options from system
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters =
                {
                    new JsonStringEnumConverter(allowIntegerValues: true)
                }
            },
            cancellationToken: cancellationToken);

        await commandExecutor.HandleAsync(command!, cancellationToken);
        

        foreach (var file in request.Form.Files.Where(f => f.Name != "command" && f.ContentType != "application/json"))
        {
            await ProcessFileAttachments(gardenerId, file, fileStorage);
        }
    }


    private static async Task ProcessFileAttachments(GardenerId gardenerId, IFormFile file, IFileStorage fileStorage)
    {
        var @params = HttpUtility.ParseQueryString(file.Name);

        var gardenId = new Guid(@params["gardenId"]!);
        var imageId = new Guid(@params["imageId"]!);
        var filename = @params["filename"]!;

        var contentType = file.ContentType;
        var stream = file.OpenReadStream();

        await fileStorage.SaveAsync(
                gardenerId,
                gardenId,
                imageId,
                filename,
                contentType,
                stream);
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
