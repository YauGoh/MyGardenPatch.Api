using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;

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

public class FileSystemFileStorage : IFileStorage
{
    private readonly string _baseFolder;

    public FileSystemFileStorage(IConfiguration configuration)
    {
        _baseFolder = configuration.GetValue<string>("StorageFolder");
    }

    public async Task SaveAsync(GardenerId gardenerId, GardenId gardenId, ImageId imageId, string filename, string contentType, Stream fileStream)
    {
        var path = Path.Join(_baseFolder, gardenerId.ToString(), gardenId.ToString(), imageId.ToString());

        Directory.CreateDirectory(path);

        var filePath = Path.Join(path, filename);

        var fileInfo = new FileInfo(filePath);

        using var file = fileInfo.OpenWrite();

        await fileStream.CopyToAsync(file);
    }
}

public class AzureBlobFileStorage : IFileStorage
{
    private readonly IOptions<AzureBlobStorageConfig> _config;

    public AzureBlobFileStorage(IOptions<AzureBlobStorageConfig> config)
    {
        _config = config;
    }

    public async Task SaveAsync(GardenerId gardenerId, GardenId gardenId, ImageId imageId, string filename, string contentType, Stream fileStream)
    {
        var serviceClient = new BlobServiceClient(_config.Value.ConnectionString);

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