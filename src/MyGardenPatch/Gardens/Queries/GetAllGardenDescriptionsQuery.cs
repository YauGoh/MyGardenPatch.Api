
namespace MyGardenPatch.Gardens.Queries;

[Role(WellKnownRoles.Gardener)]
public record GetAllGardenDescriptionsQuery() : IQuery<IEnumerable<GardenDescriptor>>;

public record GardenDescriptor(
    GardenId GardenId, 
    string Name, 
    string Description, 
    Point Center,
    Location Location,
    Uri? ImageUri, 
    string? ImageDescription) : Descriptor(Name, Description, Center, Location, ImageUri, ImageDescription);

public class GetAllGardenDescriptionsQueryHandler : IQueryHandler<GetAllGardenDescriptionsQuery, IEnumerable<GardenDescriptor>>
{
    private readonly ICurrentUserProvider _currentUser;
    private readonly IRepository<Garden, GardenId> _gardens;

    public GetAllGardenDescriptionsQueryHandler( 
        ICurrentUserProvider currentUser,
        IRepository<Garden, GardenId> gardens)
    {
        _currentUser = currentUser;
        _gardens = gardens;
    }

    public async Task<IEnumerable<GardenDescriptor>> HandleAsync(
        GetAllGardenDescriptionsQuery query, 
        CancellationToken cancellationToken = default)
    {
        var gardens = await _gardens
            .WhereAsync(
                g => g.UserId == _currentUser.UserId,
                cancellationToken);

        return gardens
            .Select(g => new GardenDescriptor(
                g.Id,
                g.Name,
                g.Description,
                g.Location.Center,
                g.Location,
                g.ImageUri,
                g.ImageDescription))
            .AsEnumerable();
    }
}

public class GetAllGardenDescriptionsQueryValidator : 
    LoggedInCurrentUserQueryValidator<GetAllGardenDescriptionsQuery>
{
    public GetAllGardenDescriptionsQueryValidator(ICurrentUserProvider currentUser) : base(currentUser)
    {
    }
}
