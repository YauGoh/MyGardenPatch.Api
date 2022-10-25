namespace MyGardenPatch.GardenBeds.Commands;


[Role(WellKnownRoles.Gardener)]
public record ReshapeGardenBedCommand(
    GardenId GardenId, 
    GardenBedId GardenBedId, 
    Shape Shape)
   : IGardenBedCommand;

public class ReshapeGardenBedCommandHandler : ICommandHandler<ReshapeGardenBedCommand>
{
    private readonly IRepository<GardenBed, GardenBedId> _gardenBeds;

    public ReshapeGardenBedCommandHandler(
        IRepository<GardenBed, GardenBedId> gardenBeds)
    {
        _gardenBeds = gardenBeds;
    }

    public async Task HandleAsync(
        ReshapeGardenBedCommand command, 
        CancellationToken cancellationToken = default)
    {
        var gardenBed = await _gardenBeds.GetAsync(
            command.GardenBedId,
            cancellationToken);

        gardenBed!.Reshape(command.Shape);

        await _gardenBeds.AddOrUpdateAsync(
            gardenBed,
            cancellationToken);
    }
}

public class ReshapeGardenBedCommandValidator : GardenBedCommandValidator<ReshapeGardenBedCommand>, ICommandValidator<ReshapeGardenBedCommand>
{
    public ReshapeGardenBedCommandValidator(
        ICurrentUserProvider currentUser,
        IRepository<Garden, GardenId> gardens,
        IRepository<GardenBed, GardenBedId> gardenBeds)
        : base(currentUser, gardens, gardenBeds)
    {
    }
}
