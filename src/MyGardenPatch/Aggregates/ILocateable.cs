using MyGardenPatch.Common;

namespace MyGardenPatch.Aggregates;

public interface ILocateable
{
    public Location Location { get; }

    void SetLocation(Location location);
}
