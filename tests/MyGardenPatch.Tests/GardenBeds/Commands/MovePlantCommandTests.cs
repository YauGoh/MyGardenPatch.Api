namespace MyGardenPatch.Tests.GardenBeds.Commands;

public class ReshapePlantCommandTests : TestBase
{
    public ReshapePlantCommandTests()
    {
        SeedWith(UserTestData.PeterParker);
        SeedWith(GardenTestData.PeterGarden);
        SeedWith(GardenBedTestData.PeterGardenBedWithCarrots);
        SetCurrentUser(UserTestData.PeterParker.Id);
    }

    [Fact]
    public async Task ReshapePlant()
    {
        var circle = Shapes.Circular_2;

        var command = new ReshapePlantCommand(
            GardenTestData.PeterGarden.Id,
            GardenBedTestData.PeterGardenBedWithCarrots.Id,
            GardenBedTestData.PeterGardenBedWithCarrots.Plants.First().Id,
            circle);

        await ExecuteCommandAsync(command);

        var gardenBed = await GetAsync<GardenBed, GardenBedId>(GardenBedTestData.PeterGardenBedWithCarrots.Id);

        gardenBed!.Plants.First().Shape.Should().BeEquivalentTo(circle);
    }
}
