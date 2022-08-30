namespace MyGardenPatch.GardenBeds.Queries;

[Role(WellKnownRoles.Gardener)]
public record GetAllGardenBedDescriptionsQuery(GardenId GardenId) : IQuery<IEnumerable<GardenBedDescriptor>>;

public record GardenBedDescriptor(
    GardenBedId GardenBedId, 
    string Name, 
    string Description, 
    Point Center,
    Location Location,
    Uri? ImageUri, 
    string? ImageDescription) : Descriptor(Name, Description, Center, Location, ImageUri, ImageDescription);

public class GetAllGardenBedDescriptionsQueryHandler : IQueryHandler<GetAllGardenBedDescriptionsQuery, IEnumerable<GardenBedDescriptor>>
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

    public async Task<IEnumerable<GardenBedDescriptor>> HandleAsync(GetAllGardenBedDescriptionsQuery query, CancellationToken cancellationToken = default)
    {
        var gardenBeds = await _gardenBeds.WhereAsync(g => g.GardenId == query.GardenId &&
                                                           g.UserId == _currentUser.UserId);

        return gardenBeds
            .Select(gb => new GardenBedDescriptor(
                gb.Id,
                gb.Name,
                gb.Description,
                gb.Location.Center,
                gb.Location,
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
