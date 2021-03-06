namespace MyGardenPatch.Gardens.Commands;

[Role(WellKnownRoles.Gardener)]
public record StartNewGardenCommand(
    string Name, 
    string Description, 
    Location Location, 
    Uri? ImageUri, 
    string? ImageDescription) : 
        ICommand, 
        INameableCommand, 
        IImageableCommand;

public class StartNewGardenCommandHandler : ICommandHandler<StartNewGardenCommand>
{
    private readonly IRepository<Garden, GardenId> _gardens;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IDateTimeProvider _dateTime;

    public StartNewGardenCommandHandler(
        IRepository<Garden, GardenId> gardens, 
        ICurrentUserProvider currentUserProvider,
        IDateTimeProvider dateTime)
    {
        _gardens = gardens;
        _currentUserProvider = currentUserProvider;
        _dateTime = dateTime;
    }

    public async Task HandleAsync(
        StartNewGardenCommand command, 
        CancellationToken cancellationToken = default)
    {
        var garden = new Garden(
            _currentUserProvider.CurrentUserId ?? throw new UserNotAuthenticatedException(),
            command.Name,
            command.Description,
            command.ImageUri,
            command.ImageDescription,
            _dateTime.Now);

        garden.SetLocation(command.Location);

        await _gardens.AddOrUpdateAsync(
            garden, 
            cancellationToken);
    }
}

public class StartNewGardenCommandValidator : 
    AbstractValidator<StartNewGardenCommand>, 
    ICommandValidator<StartNewGardenCommand>
{
    public StartNewGardenCommandValidator()
    {
        this.ValidateNameable();
        this.ValidateImageable();
    }
}
