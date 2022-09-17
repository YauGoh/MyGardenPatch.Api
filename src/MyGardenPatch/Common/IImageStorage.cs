namespace MyGardenPatch.Common;

public partial record struct ImageId: IEntityId;

public interface IImageStorage
{
    Task SaveAsync(GardenerId gardenerId, ImageId imageId, string contentType, Stream fileStream);
}
