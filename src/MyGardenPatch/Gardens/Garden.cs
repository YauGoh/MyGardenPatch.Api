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
    public Point Point { get; set; } = Point.Default;                  
    public DateTime CreatedAt { get; private set; }

    internal void Describe(string name, string description, Uri imageUri, string imageDescription)
    {
        Name = name;
        Description = description;
        ImageUri = imageUri;
        ImageDescription = imageDescription;
    }

    internal void Remove()
    {
        Raise(new GardenRemoved(Id));
    }
}
