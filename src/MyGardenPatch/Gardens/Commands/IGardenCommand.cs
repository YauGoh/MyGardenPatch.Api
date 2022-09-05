using FluentValidation;
using MyGardenPatch.Aggregates;
using MyGardenPatch.Commands;
using MyGardenPatch.Common;

namespace MyGardenPatch.Gardens.Commands;

public interface IGardenCommand : ICommand
{
    GardenId GardenId { get; }
}

public abstract class GardenCommandValidator<T> : AbstractValidator<T>, ICommandValidator<T>
   where T : IGardenCommand
{
    public GardenCommandValidator(ICurrentUserProvider currentUser, IRepository<Garden, GardenId> gardens)
    {
        // Garden should exist and belong to current user
        RuleFor(c => c.GardenId)
            .MustAsync(async (gardenId, cancellationToken) => await gardens
                .AnyAsync(
                    g => g.Id == gardenId &&
                         g.GardenerId == currentUser.GardenerId,
                    cancellationToken))
            .WithMessage("Garden does not exist");
    }
}
