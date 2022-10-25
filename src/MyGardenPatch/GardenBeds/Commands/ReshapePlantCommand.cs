namespace MyGardenPatch.GardenBeds.Commands;

[Role(WellKnownRoles.Gardener)]
public record ReshapePlantCommand(
    GardenId GardenId, 
    GardenBedId GardenBedId, 
    PlantId PlantId, 
    Shape Shape) : IPlantCommand, IShapeableCommand;

public class ReshapePlantCommandHandler : ICommandHandler<ReshapePlantCommand>
{
    private readonly IRepository<GardenBed, GardenBedId> _gardenBeds;

    public ReshapePlantCommandHandler(
        IRepository<GardenBed, GardenBedId> gardenBeds)
    {
        _gardenBeds = gardenBeds;
    }

    public async Task HandleAsync(
        ReshapePlantCommand command, 
        CancellationToken cancellationToken = default)
    {
        var gardenBed = await _gardenBeds.GetAsync(
            command.GardenBedId, 
            cancellationToken);

        gardenBed!.ReshapePlant(
            command.PlantId,
            command.Shape);

        await _gardenBeds.AddOrUpdateAsync(
            gardenBed, 
            cancellationToken);
    }
}

public class ReshpaePlantCommandValidator : PlantCommandValidator<ReshapePlantCommand>, ICommandValidator<ReshapePlantCommand>
{
    public ReshpaePlantCommandValidator(
        ICurrentUserProvider currentUser, 
        IRepository<Garden, GardenId> gardens, 
        IRepository<GardenBed, GardenBedId> gardenBeds) :
        base(currentUser, gardens, gardenBeds)
    {
        this.ValidateShape();
    }
}
