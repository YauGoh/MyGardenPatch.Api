namespace MyGardenPatch.GardenBeds.Commands;

[Role(WellKnownRoles.Gardener)]
public record AddPlantCommand(
    GardenId GardenId,
    GardenBedId GardenBedId,
    string Name,
    string Description,
    Location Location,
    Uri ImageUri,
    string ImageDescription) : IGardenBedCommand, INameableCommand, ILocateableCommand, IImageableCommand;

public class AddPlantCommandHandler : ICommandHandler<AddPlantCommand>
{
    private readonly IDateTimeProvider _dateTime;
    private readonly IRepository<GardenBed, GardenBedId> _gardenBeds;

    public AddPlantCommandHandler(
        IDateTimeProvider dateTime,
        IRepository<GardenBed, GardenBedId> gardenBeds)
    {
        _dateTime = dateTime;
        _gardenBeds = gardenBeds;
    }

    public async Task HandleAsync(
        AddPlantCommand command, 
        CancellationToken cancellationToken = default)
    {
        var gardenBed = await _gardenBeds.GetAsync(
            command.GardenBedId, 
            cancellationToken);

        gardenBed!.AddPlant(
            command.Name,
            command.Description,
            command.Location,
            command.ImageUri,
            command.ImageDescription,
            _dateTime.Now);

        await _gardenBeds.AddOrUpdateAsync(
            gardenBed, 
            cancellationToken);
    }
}

public class AddPlantCommandValidator : GardenBedCommandValidator<AddPlantCommand>
{
    public AddPlantCommandValidator(
        ICurrentUserProvider currentUser,
        IRepository<Garden, GardenId> gardens,
        IRepository<GardenBed, GardenBedId> gardenBeds)
        : base(currentUser, gardens, gardenBeds)
    {
        this.ValidateNameable();
        this.ValidateLocatable();
        this.ValidateImageable();
    }
}
