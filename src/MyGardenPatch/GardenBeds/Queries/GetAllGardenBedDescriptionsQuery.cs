namespace MyGardenPatch.GardenBeds.Queries;

[Role(WellKnownRoles.Gardener)]
public record GetAllGardenBedDescriptionsQuery(GardenId GardenId) : IQuery<IEnumerable<GardenBedDescription>>;

public record GardenBedDescription(
    GardenBedId GardenBedId, 
    string Name, 
    string Description, 
    Point Center,
    Uri? ImageUri, 
    string? ImageDescription);

public class GetAllGardenBedDescriptionsQueryHandler : IQueryHandler<GetAllGardenBedDescriptionsQuery, IEnumerable<GardenBedDescription>>
{
    private readonly ICurrentUserProvider _currentUser;
    private readonly IRepository<GardenBed, GardenBedId> _gardenBeds;

    public GetAllGardenBedDescriptionsQueryHandler(
        ICurrentUserProvider currentUser,
        IRepository<GardenBed, GardenBedId> gardenBeds)
    {
        _currentUser = currentUser;
        _gardenBeds = gardenBeds;
    }

    public async Task<IEnumerable<GardenBedDescription>> HandleAsync(GetAllGardenBedDescriptionsQuery query, CancellationToken cancellationToken = default)
    {
        var gardenBeds = await _gardenBeds.WhereAsync(g => g.GardenId == query.GardenId &&
                                                           g.UserId == _currentUser.CurrentUserId);

        return gardenBeds
            .Select(gb => new GardenBedDescription(
                gb.Id,
                gb.Name,
                gb.Description,
                gb.Location.Center,
                gb.ImageUri,
                gb.ImageDescription))
            .AsEnumerable();
    }
}

public class GetAllGardenBedDescriptionsQueryValidator :
    LoggedInCurrentUserQueryValidator<GetAllGardenBedDescriptionsQuery>
{
    public GetAllGardenBedDescriptionsQueryValidator(ICurrentUserProvider currentUser) 
        : base(currentUser)
    {
    }
}
