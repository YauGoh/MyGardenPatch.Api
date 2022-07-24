namespace MyGardenPatch.GardenBeds.Queries;

public abstract class GardenBedQueryValidator<TQuery> : LoggedInCurrentUserQueryValidator<TQuery>
    where TQuery : IGardenBedQuery
{
    public GardenBedQueryValidator(
        ICurrentUserProvider currentUser, 
        IRepository<Garden, GardenId> gardens) : base(currentUser)
    {
        RuleFor(q => q.GardenId)
            .MustAsync((gardenId, cancellationToken) => gardens
                .AnyAsync(g => g.Id == gardenId &&
                               g.UserId == currentUser.CurrentUserId))
            .WithMessage("Garden doesn't exist");
    }
}
