namespace MyGardenPatch.Common;

public interface ICurrentUserProvider
{
    GardenerId? GardenerId { get; }

    string? EmailAddress { get; }

    string? Name { get; }
}
