namespace MyGardenPatch.WebApi.Tests.TestFixtureExtensions;

internal static class PlantExtensions
{
    internal static TestFixture WithPlantId(this TestFixture fixture, PlantId plantId)
    {
        fixture.SetState(plantId);
        return fixture;
    }

    internal static PlantId GetPlantId(this TestFixture fixture)
        => fixture.GetState<PlantId>();
}
