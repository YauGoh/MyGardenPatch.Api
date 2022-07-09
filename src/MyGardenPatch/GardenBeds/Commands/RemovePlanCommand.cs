using MyGardenPatch.Aggregates;
using MyGardenPatch.Commands;
using MyGardenPatch.Common;
using MyGardenPatch.Gardens;

namespace MyGardenPatch.GardenBeds.Commands;

public record class RemovePlantCommand(
    GardenId GardenId, 
    GardenBedId GardenBedId, 
    PlantId PlantId) : IPlantCommand;

public class RemovePlantCommandHandler : ICommandHandler<RemovePlantCommand>
{
    private readonly IRepository<GardenBed, GardenBedId> _gardenBeds;

    public RemovePlantCommandHandler(
        IRepository<GardenBed, GardenBedId> gardenBeds)
    {
        _gardenBeds = gardenBeds;
    }


    public async Task HandleAsync(
        RemovePlantCommand command, 
        CancellationToken cancellationToken = default)
    {
        var gardenBed = await _gardenBeds.GetAsync(
            command.GardenBedId, 
            cancellationToken);

        gardenBed!.RemovePlant(command.PlantId);

        await _gardenBeds.AddOrUpdateAsync(
            gardenBed, 
            cancellationToken);
    }
}

public class RemovePlantCommandValidator : PlantCommandValidator<RemovePlantCommand>, ICommandValidator<RemovePlantCommand>
{
    public RemovePlantCommandValidator(
        ICurrentUserProvider currentUser, 
        IRepository<Garden, GardenId> gardens, 
        IRepository<GardenBed, GardenBedId> gardenBeds)
        : base(currentUser, gardens, gardenBeds)
    {
    }
}
