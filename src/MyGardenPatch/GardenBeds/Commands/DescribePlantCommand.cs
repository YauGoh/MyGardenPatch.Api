using MyGardenPatch.Aggregates;
using MyGardenPatch.Commands;
using MyGardenPatch.Common;
using MyGardenPatch.Gardens;

namespace MyGardenPatch.GardenBeds.Commands
{
    public record DescribePlantCommand(GardenId GardenId, GardenBedId GardenBedId, PlantId PlantId, string Name, string Description, Uri ImageUri, string ImageDescription) :
        IPlantCommand,
        INameableCommand,
        IImageableCommand;

    public class DescribePlantCommandHandler : ICommandHandler<DescribePlantCommand>
    {
        private readonly IRepository<GardenBed, GardenBedId> _gardenBeds;

        public DescribePlantCommandHandler(
            IRepository<GardenBed, GardenBedId> gardenBeds)
        {
            _gardenBeds = gardenBeds;
        }

        public async Task HandleAsync(
            DescribePlantCommand command, 
            CancellationToken cancellationToken = default)
        {
            var gardenBed = await _gardenBeds.GetAsync(
                command.GardenBedId, 
                cancellationToken);

            gardenBed!.DescribePlant(
                command.PlantId, 
                command.Name, 
                command.Description, 
                command.ImageUri, 
                command.ImageDescription);

            await _gardenBeds.AddOrUpdateAsync(
                gardenBed, 
                cancellationToken);
        }
    }

    public class DescribePlantCommandValidator : PlantCommandValidator<DescribePlantCommand>, ICommandValidator<DescribePlantCommand>
    {
        public DescribePlantCommandValidator(
            ICurrentUserProvider currentUser, 
            IRepository<Garden, GardenId> gardens, 
            IRepository<GardenBed, GardenBedId> gardenBeds) : base(currentUser, gardens, gardenBeds)
        {
            this.ValidateImageable();
            this.ValidateNameable();
        }
    }
}
