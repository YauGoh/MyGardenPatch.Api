
namespace MyGardenPatch.Gardens.Queries;

[Role(WellKnownRoles.Gardener)]
public record GetAllGardenDescriptionsQuery() : IQuery<IEnumerable<GardenDescriptor>>;

public record GardenDescriptor(
    GardenId GardenId, 
    GardenerId GardenerId,
    string Name, 
    string Description, 
    Point Center,
    Uri? ImageUri, 
    string? ImageDescription) : Descriptor(Name, Description, Center, ImageUri, ImageDescription);

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
                g => g.GardenerId == _currentUser.GardenerId,
                cancellationToken);

        return gardens
            .Select(g => new GardenDescriptor(
                g.Id,
                g.GardenerId,
                g.Name,
                g.Description,
                g.Center,
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
