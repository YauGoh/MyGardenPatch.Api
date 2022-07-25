namespace MyGardenPatch.GardenBeds.Commands;

[Role(WellKnownRoles.Gardener)]
public record RemoveGardenBedCommand(
    GardenId GardenId, 
    GardenBedId GardenBedId) : IGardenBedCommand;

public class RemoveGardenBedCommandHandler : ICommandHandler<RemoveGardenBedCommand>
{
    private readonly IRepository<GardenBed, GardenBedId> _gardenBeds;

    public RemoveGardenBedCommandHandler(
        IRepository<GardenBed, GardenBedId> gardenBeds)
    {
        _gardenBeds = gardenBeds;
    }

    public async Task HandleAsync(
        RemoveGardenBedCommand command, 
        CancellationToken cancellationToken = default)
    {
        var gardenBed = await _gardenBeds.GetAsync(
            command.GardenBedId, 
            cancellationToken);

        gardenBed!.Remove();

        await _gardenBeds.DeleteAsync(
            gardenBed, 
            cancellationToken);
    }
}

public class RemoveGardenBedCommandValidator : GardenBedCommandValidator<RemoveGardenBedCommand>, ICommandValidator<RemoveGardenBedCommand>
{
    public RemoveGardenBedCommandValidator(
        ICurrentUserProvider currentUser,
        IRepository<Garden, GardenId> gardens,
        IRepository<GardenBed, GardenBedId> gardenBeds) : base(currentUser, gardens, gardenBeds)
    {
    }
}
