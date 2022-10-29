namespace MyGardenPatch.Gardens.Commands;

[Role(WellKnownRoles.Gardener)]
public record StartNewGardenCommand(
    GardenId GardenId,
    string Name, 
    string Description, 
    Point Center, 
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
    private readonly IFileAttachments _fileAttachments;
    private readonly IFileStorage _fileStorage;

    public StartNewGardenCommandHandler(
        IRepository<Garden, GardenId> gardens, 
        ICurrentUserProvider currentUserProvider,
        IDateTimeProvider dateTime,
        IFileAttachments fileAttachments,
        IFileStorage fileStorage)
    {
        _gardens = gardens;
        _currentUserProvider = currentUserProvider;
        _dateTime = dateTime;
        _fileAttachments = fileAttachments;
        _fileStorage = fileStorage;
    }

    public async Task HandleAsync(
        StartNewGardenCommand command, 
        CancellationToken cancellationToken = default)
    {
        var garden = new Garden(
            command.GardenId,
            _currentUserProvider.GardenerId ?? throw new UserNotAuthenticatedException(),
            command.Name,
            command.Description,
            command.ImageUri,
            command.ImageDescription,
            _dateTime.Now)
        {
            Center = command.Center
        };

        await ProcessFileAttachments();

        await _gardens.AddOrUpdateAsync(
            garden, 
            cancellationToken);
    }

    private async Task ProcessFileAttachments()
    {
        foreach(var file in _fileAttachments.GetAll())
        {
            await _fileStorage.SaveAsync(
                _currentUserProvider.GardenerId ?? throw new UserNotAuthenticatedException(),
                file.GardenId,
                file.ImageId,
                file.Filename,
                file.ContentType,
                file.Stream);
        }
    }
}

public class StartNewGardenCommandValidator : 
    AbstractValidator<StartNewGardenCommand>, 
    ICommandValidator<StartNewGardenCommand>
{
    public StartNewGardenCommandValidator()
    {
        RuleFor(command => command.GardenId)
            .NotEmpty()
            .WithMessage("A valid garden id is required");

        this.ValidateNameable();
        this.ValidateImageable();
    }
}
