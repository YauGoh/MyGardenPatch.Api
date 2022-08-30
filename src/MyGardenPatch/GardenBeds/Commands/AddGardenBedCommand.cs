namespace MyGardenPatch.GardenBeds.Commands;

[Role(WellKnownRoles.Gardener)]
public record AddGardenBedCommand(
    GardenId GardenId, 
    string Name, 
    string Description, 
    Location Location, 
    Uri? ImageUri, 
    string? ImageDescription)
    : ICommand, INameableCommand, ILocateableCommand, IImageableCommand;

public class AddGardenBedCommandHandler : ICommandHandler<AddGardenBedCommand>
{
    private readonly IRepository<GardenBed, GardenBedId> _gardenBeds;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IDateTimeProvider _dateTime;

    public AddGardenBedCommandHandler(
        IRepository<GardenBed, GardenBedId> gardenBeds, 
        ICurrentUserProvider currentUserProvider,
        IDateTimeProvider dateTime)
    {
        _gardenBeds = gardenBeds;
        _currentUserProvider = currentUserProvider;
        _dateTime = dateTime;
    }

    public async Task HandleAsync(
        AddGardenBedCommand command, 
        CancellationToken cancellationToken = default)
    {
        var gardenBed = new GardenBed(
            _currentUserProvider.UserId!.Value,
            command.GardenId,
            command.Name,
            command.Description,
            command.ImageUri,
            command.ImageDescription,
            _dateTime.Now);

        gardenBed.SetLocation(command.Location);

        await _gardenBeds.AddOrUpdateAsync(
            gardenBed, 
            cancellationToken);
    }
}

public class AddGardenBedCommandValidator : AbstractValidator<AddGardenBedCommand>, ICommandValidator<AddGardenBedCommand>
{
    public AddGardenBedCommandValidator(
        IRepository<Garden, GardenId> gardens, 
        ICurrentUserProvider currentUserProvider)
    {
        // Garden should exist and belong to current user
        RuleFor(c => c.GardenId)
            .MustAsync(async (gardenId, cancellationToken) => await gardens
                .AnyAsync(
                    g => g.Id == gardenId &&
                         g.UserId == currentUserProvider.UserId,
                    cancellationToken))
            .WithMessage("Garden does not exist");

        this.ValidateNameable();

        this.ValidateLocatable();

        this.ValidateImageable();
    }
}
