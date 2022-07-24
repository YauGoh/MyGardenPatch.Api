namespace MyGardenPatch.GardenBeds.Queries;

[Role(WellKnownRoles.Gardener)]
public record GetAllPlantDescriptionsQuery(
    GardenId GardenId,
    GardenBedId GardenBedId) : IQuery<IEnumerable<PlantDescription>>, IPlantQuery;

public record PlantDescription(
    PlantId PlantId,
    string Name,
    string Description,
    Uri? ImageUri,
    string? ImageDescription,
    Point Center);

public class GetAllPlantDescriptionsQueryHandler : IQueryHandler<GetAllPlantDescriptionsQuery, IEnumerable<PlantDescription>>
{
    private readonly ICurrentUserProvider _currentUser;
    private readonly IRepository<GardenBed, GardenBedId> _gardenBeds;

    public GetAllPlantDescriptionsQueryHandler(
        ICurrentUserProvider currentUser,
        IRepository<GardenBed, GardenBedId> gardenBeds)
    {
        _currentUser = currentUser;
        _gardenBeds = gardenBeds;
    }

    public async Task<IEnumerable<PlantDescription>> HandleAsync(
        GetAllPlantDescriptionsQuery query, 
        CancellationToken cancellationToken = default)
    {
        var gardenBed = await _gardenBeds.GetByExpressionAsync(
            gb => gb.Id == query.GardenBedId &&
                  gb.GardenId == query.GardenId &&
                  gb.UserId == _currentUser.CurrentUserId,
            cancellationToken);

        return gardenBed!.Plants
            .Select(
                p => new PlantDescription(
                    p.Id,
                    p.Name,
                    p.Description,
                    p.ImageUri,
                    p.ImageDescription,
                    p.Location.Center))
            .AsEnumerable();
    }
}

public class GetAllPlantDescriptionQueryValidator : PlantQueryValidator<GetAllPlantDescriptionsQuery>
{
    public GetAllPlantDescriptionQueryValidator(
        ICurrentUserProvider currentUser, 
        IRepository<Garden, GardenId> gardens, 
        IRepository<GardenBed, GardenBedId> gardenBeds) : base(currentUser, gardens, gardenBeds)
    {

    }
}
