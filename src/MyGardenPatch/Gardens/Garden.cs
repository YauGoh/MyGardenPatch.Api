namespace MyGardenPatch.Gardens;

public partial record struct GardenId : IEntityId { }

public class Garden : GardenerOwnedAggregate<GardenId>, INameable, ILocateable
{
    public Garden(GardenId id, GardenerId gardenerId, string name, string description, Uri? imageUri, string? imageDescription, DateTime createdAt)
        : base(id, gardenerId)
    {
        Name = name;
        Description = description;
        ImageUri = imageUri;
        ImageDescription = imageDescription;
        CreatedAt = createdAt;
    }

    public Garden(GardenerId gardenerId, string name, string description, Uri? imageUri, string? imageDescription, DateTime createdAt)
        : this(new(), gardenerId, name, description, imageUri, imageDescription, createdAt)
    { }


    public string Name { get; private set; }
    public string Description { get; private set; }
    public Uri? ImageUri { get; private set; }
    public string? ImageDescription { get; private set; }
    public Location Location { get; private set; } = Location.Default;

    public DateTime CreatedAt { get; private set; }

    Location ILocateable.Location => throw new NotImplementedException();

    internal void Describe(string name, string description, Uri imageUri, string imageDescription)
    {
        Name = name;
        Description = description;
        ImageUri = imageUri;
        ImageDescription = imageDescription;
    }

    internal void Move(Transformation transformation, bool moveGardenBeds)
    {
        SetLocation(Location.Transform(transformation));

        Raise(new GardenMoved(Id, transformation, moveGardenBeds));
    }

    internal void Remove()
    {
        Raise(new GardenRemoved(Id));
    }

    public void SetLocation(Location location) => Location = location;
}
