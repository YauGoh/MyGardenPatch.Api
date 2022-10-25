namespace MyGardenPatch.GardenBeds;

public partial record struct PlantId : IEntityId { }

public class Plant : Entity<PlantId>, INameable, IShapeable
{
    public Plant(PlantId id, string name, string description, Uri? imageUri, string? imageDescription, DateTime createdAt) : base(id)
    {
        Name = name;
        Description = description;
        ImageUri = imageUri;
        ImageDescription = imageDescription;
        CreatedAt = createdAt;
    }

    public Plant(string name, string description, Uri? imageUri, string? imageDescription, DateTime createdAt)
        : this(new(), name, description, imageUri, imageDescription, createdAt)
    {
    }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public Shape Shape { get; set; }

    public Uri? ImageUri { get; private set; }

    public string? ImageDescription { get; private set; }

    public DateTime CreatedAt { get; set; }

    internal void Describe(string name, string description, Uri imageUri, string imageDescription)
    {
        Name = name;
        Description = description;
        ImageUri = imageUri;
        ImageDescription = imageDescription;
    }
}
