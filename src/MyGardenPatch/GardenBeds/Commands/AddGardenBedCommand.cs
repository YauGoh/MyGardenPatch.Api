namespace MyGardenPatch.GardenBeds.Commands;

[Role(WellKnownRoles.Gardener)]
public record AddGardenBedCommand(
    GardenBedId GardenBedId,
    GardenId GardenId, 
    string Name, 
    string Description, 
    Shape Shape, 
    Uri? ImageUri, 
    string? ImageDescription)
    : ICommand, INameableCommand, IShapeableCommand, IImageableCommand;

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
            command.GardenBedId,
            _currentUserProvider.GardenerId!.Value,
            command.GardenId,
            command.Name,
            command.Description,
            command.ImageUri,
            command.ImageDescription,
            _dateTime.Now)
        {
            Shape = command.Shape
        };

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
                         g.GardenerId == currentUserProvider.GardenerId,
                    cancellationToken))
            .WithMessage("Garden does not exist");

        this.ValidateNameable();

        this.ValidateShape();

        this.ValidateImageable();
    }
}
