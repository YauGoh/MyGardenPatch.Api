using MyGardenPatch.Aggregates;
using MyGardenPatch.Commands;
using MyGardenPatch.Common;
using MyGardenPatch.Gardens;

namespace MyGardenPatch.GardenBeds.Commands;

public record RemoveGardenBebCommand(
    GardenId GardenId, 
    GardenBedId GardenBedId) : IGardenBedCommand;

public class RemoveGardenBedCommandHandler : ICommandHandler<RemoveGardenBebCommand>
{
    private readonly IRepository<GardenBed, GardenBedId> _gardenBeds;

    public RemoveGardenBedCommandHandler(
        IRepository<GardenBed, GardenBedId> gardenBeds)
    {
        _gardenBeds = gardenBeds;
    }

    public async Task HandleAsync(
        RemoveGardenBebCommand command, 
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

public class RemoveGardenBedCommandValidator : GardenBedCommandValidator<RemoveGardenBebCommand>, ICommandValidator<RemoveGardenBebCommand>
{
    public RemoveGardenBedCommandValidator(
        ICurrentUserProvider currentUser,
        IRepository<Garden, GardenId> gardens,
        IRepository<GardenBed, GardenBedId> gardenBeds) : base(currentUser, gardens, gardenBeds)
    {
    }
}
