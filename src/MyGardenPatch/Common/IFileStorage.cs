using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace MyGardenPatch.Common;

public partial record struct ImageId: IEntityId;

public interface IFileStorage
{
    Task SaveAsync(GardenerId gardenerId, GardenId gardenId, ImageId imageId, string filename, string contentType, Stream fileStream);
}

public class StubFileStorage : IFileStorage
{
    public Task SaveAsync(GardenerId gardenerId, GardenId gardenId, ImageId imageId, string filename, string contentType, Stream fileStream)
    {
        return Task.CompletedTask;
    }
}

public class AzureBlobFileStorage : IFileStorage
{
    private readonly AzureBlobStorageConfig _config;

    public AzureBlobFileStorage(AzureBlobStorageConfig config)
    {
        _config = config;
    }

    public async Task SaveAsync(GardenerId gardenerId, GardenId gardenId, ImageId imageId, string filename, string contentType, Stream fileStream)
    {
        var serviceClient = new BlobServiceClient(_config.ConnectionString);

        var container = await serviceClient.CreateBlobContainerAsync($"{gardenerId}/{gardenId}/{imageId}");

        var blobClient = container.Value.GetBlobClient(filename);

        await blobClient.UploadAsync(
            fileStream, 
            new BlobUploadOptions 
            { 
                HttpHeaders = new BlobHttpHeaders 
                { 
                    ContentType = contentType 
                } 
            }
        );
    }
}