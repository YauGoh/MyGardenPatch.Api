namespace MyGardenPatch.Common
{
    public abstract record Descriptor(
        string Name, 
        string Description, 
        Point Center, 
        Uri? ImageUri, 
        string? ImageDescription);
}
