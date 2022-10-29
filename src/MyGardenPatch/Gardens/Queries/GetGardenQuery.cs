namespace MyGardenPatch.Gardens.Queries;

[Role(WellKnownRoles.Gardener)]
public record GetGardenMapQuery(GardenId GardenId) : IQuery<GardenMap>;

public record GardenMap : GardenDescriptor
{
    public GardenMap(
        GardenId GardenId, 
        GardenerId GardenerId, 
        string Name, 
        string Description, 
        Point Center, 
        Uri? ImageUri, 
        string? ImageDescription,
        IEnumerable<GardenBedMapItem> gardenBeds) : base(GardenId, GardenerId, Name, Description, Center, ImageUri, ImageDescription)
    {
        GardenBeds = gardenBeds;
    }

    public IEnumerable<GardenBedMapItem> GardenBeds { get; }
}

public record GardenBedMapItem : GardenBedDescriptor
{
    public GardenBedMapItem(
        GardenBedId GardenBedId, 
        string Name, 
        string Description, 
        Point Center, 
        Shape Shape, 
        Uri? ImageUri, 
        string? ImageDescription,
        IEnumerable<PlantMapItem> plants) : base(GardenBedId, Name, Description, Center, Shape, ImageUri, ImageDescription)
    {
        Plants = plants;
    }

    public IEnumerable<PlantMapItem> Plants { get; }
}

public record PlantMapItem : PlantDescription
{
    public PlantMapItem(
        PlantId PlantId, 
        string Name, 
        string Description, 
        Point Center, 
        Shape Shape, 
        Uri? ImageUri, 
        string? ImageDescription) : base(PlantId, Name, Description, Center, Shape, ImageUri, ImageDescription)
    {
    }
}

public class GetGardenMapCommandQueryHandler : IQueryHandler<GetGardenMapQuery, GardenMap>
{
    private readonly IRepository<Garden, GardenId> _gardens;
    private readonly IRepository<GardenBed, GardenBedId> _gardenBeds;

    public GetGardenMapCommandQueryHandler(IRepository<Garden, GardenId> gardens, IRepository<GardenBed, GardenBedId> gardenBeds)
    {
        _gardens = gardens;
        _gardenBeds = gardenBeds;
    }

    public async Task<GardenMap> HandleAsync(GetGardenMapQuery query, CancellationToken cancellationToken = default)
    {
        var garden = await _gardens.GetAsync(query.GardenId, cancellationToken)!;

        var gardenBeds = await _gardenBeds.WhereAsync(gb => gb.GardenId == query.GardenId, cancellationToken);

        return new GardenMap(
            garden!.Id,
            garden.GardenerId,
            garden.Name,
            garden.Description,
            garden.Center,
            garden.ImageUri,
            garden.ImageDescription,
            gardenBeds.Select(gb =>
                new GardenBedMapItem(
                    gb.Id,
                    gb.Name,
                    gb.Description,
                    gb.Shape.Point,
                    gb.Shape,
                    gb.ImageUri,
                    gb.ImageDescription,
                    gb.Plants.Select(p =>
                        new PlantMapItem(
                            p.Id,
                            p.Name,
                            p.Description,
                            p.Shape.Point,
                            p.Shape,
                            p.ImageUri,
                            p.ImageDescription)))));
    }
}

public class GatGardenMapQueryValidator : LoggedInCurrentUserQueryValidator<GetGardenMapQuery>
{
    public GatGardenMapQueryValidator(IRepository<Garden, GardenId> gardens, ICurrentUserProvider currentUser) : base(currentUser)
    {
        RuleFor(q => q.GardenId)
            .MustAsync((gardenId, cancellationToken) => gardens.AnyAsync(gb => gb.Id == gardenId, cancellationToken))
            .WithMessage("Garden does not exist");
    }
}
