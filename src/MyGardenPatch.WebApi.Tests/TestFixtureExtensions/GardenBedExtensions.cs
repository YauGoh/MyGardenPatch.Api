namespace MyGardenPatch.WebApi.Tests.TestFixtureExtensions;

internal static class GardenBedExtensions
{
    internal static TestFixture WithGardenBedId(this TestFixture fixture, GardenBedId gardenBedId)
    {
        fixture.SetState(gardenBedId);
        return fixture;
    }

    internal static GardenBedId GetGardenBedId(this TestFixture fixture) 
        => fixture.GetState<GardenBedId>();
}
