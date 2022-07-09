using MyGardenPatch.Aggregates;
using MyGardenPatch.Common;

namespace MyGardenPatch.GardenBeds;

public partial record struct PlantId : IEntityId { }

public class Plant : Entity<PlantId>, INameable, ILocateable
{
    public Plant(PlantId id, string name, string description, Uri imageUri, string imageDescription, DateTime createdAt) : base(id)
    {
        Name = name;
        Description = description;
        ImageUri = imageUri;
        ImageDescription = imageDescription;
        CreatedAt = createdAt;
    }

    public Plant(string name, string description, Uri imageUri, string imageDescription, DateTime createdAt)
        : this(new(), name, description, imageUri, imageDescription, createdAt)
    {
    }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public Location Location { get; private set; } = Location.Default;

    public Uri ImageUri { get; private set; }

    public string ImageDescription { get; private set; }

    public DateTime CreatedAt { get; set; }

    Location ILocateable.Location => throw new NotImplementedException();

    internal void Describe(string name, string description, Uri imageUri, string imageDescription)
    {
        Name = name;
        Description = description;
        ImageUri = imageUri;
        ImageDescription = imageDescription;
    }

    public void SetLocation(Location location) => Location = location;

    internal void Move(Transformation transformation)
    {
        SetLocation(Location.Transform(transformation));
    }
}
