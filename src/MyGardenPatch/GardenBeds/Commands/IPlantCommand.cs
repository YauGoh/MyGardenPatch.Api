using FluentValidation;
using MyGardenPatch.Aggregates;
using MyGardenPatch.Commands;
using MyGardenPatch.Common;
using MyGardenPatch.Gardens;

namespace MyGardenPatch.GardenBeds.Commands
{
    public interface IPlantCommand : IGardenBedCommand, ICommand
    {
        PlantId PlantId { get; }
    }

    public abstract class PlantCommandValidator<T> : GardenBedCommandValidator<T>, ICommandValidator<T>
        where T : IPlantCommand
    {
        public PlantCommandValidator(ICurrentUserProvider currentUser, IRepository<Garden, GardenId> gardens, IRepository<GardenBed, GardenBedId> gardenBeds)
            : base(currentUser, gardens, gardenBeds)
        {
            RuleFor(c => c.PlantId)
                .MustAsync(async (command, plantId, _, cancellationToken) => await gardenBeds
                    .AnyAsync(
                        g => g.Id == command.GardenBedId &&
                             g.GardenId == command.GardenId &&
                             g.Plants.Any(p => p.Id == plantId) &&
                             g.UserId == currentUser.CurrentUserId,
                        cancellationToken))
                .WithMessage("Plant does not exist");
        }
    }
}
