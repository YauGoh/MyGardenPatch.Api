namespace MyGardenPatch.GardenBeds;

public partial record struct GardenBedId : IEntityId { }

public class GardenBed : GardenerOwnedAggregate<GardenBedId>, INameable, ILocateable
{
    public GardenBed(GardenBedId id, GardenerId gardenerId, GardenId gardenId, string name, string description, Uri? imageUri, string? imageDescription, DateTime createdAt)
        : base(id, gardenerId)
    {
        GardenId = gardenId;
        Name = name;
        Description = description;
        ImageUri = imageUri;
        ImageDescription = imageDescription;
        CreatedAt = createdAt;

        Plants = new HashSet<Plant>();
    }

    public GardenBed(GardenerId gardenerId, GardenId gardenId, string name, string description, Uri? imageUri, string? imageDescription, DateTime createdAt)
        : this(new(), gardenerId, gardenId, name, description, imageUri, imageDescription, createdAt)
    {
        Raise(new GardenBedAdded(gardenerId, gardenId, Id, createdAt));
    }

    public GardenId GardenId { get; private set; }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public Location Location { get; private set; } = Location.Default;

    public Uri? ImageUri { get; private set; }

    public string? ImageDescription { get; private set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<Plant> Plants { get; private set; }

    Location ILocateable.Location => throw new NotImplementedException();

    public void SetLocation(Location location) => Location = location;

    public void AddPlant(string name, string description, Location location, Uri? imageUri, string? imageDescription, DateTime createdAt)
    {
        var plant = new Plant(name, description, imageUri, imageDescription, createdAt);
        plant.SetLocation(location);

        this.Plants.Add(plant);

        Raise(new PlantAdded(GardenId, Id, plant.Id, createdAt));
    }

    internal void Describe(string name, string description, Uri? imageUri, string? imageDescription)
    {
        Name = name;
        Description = description;
        ImageUri = imageUri;
        ImageDescription = imageDescription;
    }

    internal void DescribePlant(PlantId plantId, string name, string description, Uri imageUri, string imageDescription)
    {
        Plants.Single(p => p.Id == plantId).Describe(name, description, imageUri, imageDescription);
    }


    public void Move(Transformation transformation)
    {
        SetLocation(Location.Transform(transformation));

        foreach (var plant in Plants)
        {
            plant.Move(transformation);
        }
    }

    internal void MovePlant(PlantId plantId, Transformation transformation)
    {
        Plants.First(p => p.Id == plantId).Move(transformation);
    }

    public void Remove()
    {
        Raise(new GardenBedRemoved(GardenId, Id));
    }

    internal void RemovePlant(PlantId plantId)
    {
        var plant = Plants.First(p => p.Id == plantId);

        Plants.Remove(plant);

        Raise(new PlantRemoved(GardenId, Id, plant.Id));
    }
}
