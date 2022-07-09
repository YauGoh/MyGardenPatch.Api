using MyGardenPatch.Aggregates;
using MyGardenPatch.Commands;
using MyGardenPatch.Common;
using MyGardenPatch.Gardens;

namespace MyGardenPatch.GardenBeds.Commands;

public record MovePlantCommand(
    GardenId GardenId, 
    GardenBedId GardenBedId, 
    PlantId PlantId, 
    Transformation Transformation) : IPlantCommand;

public class MovePlantCommandHandler : ICommandHandler<MovePlantCommand>
{
    private readonly IRepository<GardenBed, GardenBedId> _gardenBeds;

    public MovePlantCommandHandler(
        IRepository<GardenBed, GardenBedId> gardenBeds)
    {
        _gardenBeds = gardenBeds;
    }

    public async Task HandleAsync(
        MovePlantCommand command, 
        CancellationToken cancellationToken = default)
    {
        var gardenBed = await _gardenBeds.GetAsync(
            command.GardenBedId, 
            cancellationToken);

        gardenBed!.MovePlant(
            command.PlantId, 
            command.Transformation);

        await _gardenBeds.AddOrUpdateAsync(
            gardenBed, 
            cancellationToken);
    }
}

public class MovePlantCommandValidator : PlantCommandValidator<MovePlantCommand>, ICommandValidator<MovePlantCommand>
{
    public MovePlantCommandValidator(
        ICurrentUserProvider currentUser, 
        IRepository<Garden, GardenId> gardens, 
        IRepository<GardenBed, GardenBedId> gardenBeds) :
        base(currentUser, gardens, gardenBeds)
    {
    }
}
