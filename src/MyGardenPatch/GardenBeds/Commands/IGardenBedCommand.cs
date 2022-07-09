using FluentValidation;
using MyGardenPatch.Aggregates;
using MyGardenPatch.Commands;
using MyGardenPatch.Common;
using MyGardenPatch.Gardens;

namespace MyGardenPatch.GardenBeds.Commands;

public interface IGardenBedCommand : ICommand
{
    GardenId GardenId { get; }
    GardenBedId GardenBedId { get; }
}

public abstract class GardenBedCommandValidator<T> : AbstractValidator<T>, ICommandValidator<T>
    where T : IGardenBedCommand
{
    public GardenBedCommandValidator(
        ICurrentUserProvider currentUser, 
        IRepository<Garden, GardenId> gardens, 
        IRepository<GardenBed, GardenBedId> gardenBeds)
    {
        // Garden should exist and belong to current user
        RuleFor(c => c.GardenId)
            .MustAsync(async (gardenId, cancellationToken) => await gardens
                .AnyAsync(
                    g => g.Id == gardenId &&
                         g.UserId == currentUser.CurrentUserId,
                    cancellationToken))
            .WithMessage("Garden does not exist");

        RuleFor(c => c.GardenBedId)
            .MustAsync(async (command, gardenBedId, _, cancellationToken) => await gardenBeds
                .AnyAsync(
                    g => g.Id == gardenBedId &&
                         g.GardenId == command.GardenId &&
                         g.UserId == currentUser.CurrentUserId,
                    cancellationToken))
            .WithMessage("Garden bed does not exist");
    }
}
