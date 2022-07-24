namespace MyGardenPatch.Common
{
    public abstract record Descriptor(
        string Name, 
        string Description, 
        Point Center, 
        Location Location, 
        Uri? ImageUri, 
        string? ImageDescription);
}
