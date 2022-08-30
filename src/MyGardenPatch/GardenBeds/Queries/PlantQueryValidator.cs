namespace MyGardenPatch.GardenBeds.Queries;

public abstract class PlantQueryValidator<TQuery> : GardenBedQueryValidator<TQuery>
    where TQuery : IPlantQuery
{
    protected PlantQueryValidator(
        ICurrentUserProvider currentUser, 
        IRepository<Garden, GardenId> gardens,
        IRepository<GardenBed, GardenBedId> gardenBeds) : base(currentUser, gardens)
    {
        RuleFor(q => q.GardenBedId)
            .MustAsync((gardenBedId, cancellationToken) => gardenBeds
                .AnyAsync(g => g.Id == gardenBedId &&
                               g.UserId == currentUser.UserId))
            .WithMessage("Garden bed doesn't exist");
    }
}
