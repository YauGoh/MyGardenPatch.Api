namespace MyGardenPatch.Configurations
{
    public record AzureBlobStorageConfig
    {
        public string ConnectionString { get; init; }
    }
}
