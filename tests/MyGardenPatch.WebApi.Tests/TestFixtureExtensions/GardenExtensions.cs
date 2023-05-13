namespace MyGardenPatch.WebApi.Tests.TestFixtureExtensions;

internal static class GardenExtensions
{
    public static TestFixture WithGardenId(this TestFixture fixture, GardenId gardenId)
    { 
        fixture.SetState<GardenId>(gardenId);

        return fixture;
    }

    public static GardenId GetGardenId(this TestFixture fixture)
    {
        return fixture.GetState<GardenId>();
    }
}
