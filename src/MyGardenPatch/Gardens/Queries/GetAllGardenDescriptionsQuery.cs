namespace MyGardenPatch.Gardens.Queries;

[Role(WellKnownRoles.Gardener)]
public record GetAllGardenDescriptionsQuery() : IQuery<IEnumerable<GardenDescription>>;

public record GardenDescription(
    GardenId GardenId, 
    string Name, 
    string Description, 
    Point Center,
    Uri? ImageUri, 
    string? ImageDescription);

public class GetAllGardenDescriptionsQueryHandler : IQueryHandler<GetAllGardenDescriptionsQuery, IEnumerable<GardenDescription>>
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

    public async Task<IEnumerable<GardenDescription>> HandleAsync(
        GetAllGardenDescriptionsQuery query, 
        CancellationToken cancellationToken = default)
    {
        var gardens = await _gardens
            .WhereAsync(
                g => g.UserId == _currentUser.CurrentUserId,
                cancellationToken);

        return gardens
            .Select(g => new GardenDescription(
                g.Id,
                g.Name,
                g.Description,
                g.Location.Center,
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
