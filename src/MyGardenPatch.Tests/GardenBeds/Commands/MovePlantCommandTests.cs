namespace MyGardenPatch.Tests.GardenBeds.Commands;

public class MovePlantCommandTests : TestBase
{
    public MovePlantCommandTests()
    {
        SeedWith(UserTestData.PeterParker);
        SeedWith(GardenTestData.PeterGarden);
        SeedWith(GardenBedTestData.PeterGardenBedWithCarrots);
        SetCurrentUser(UserTestData.PeterParker.Id);
    }

    [Theory]
    [InlineData(Transformations.Identity, "1,2")]
    [InlineData(Transformations.Move1x, "2,2")]
    [InlineData(Transformations.Move1y, "1,3")]
    [InlineData(Transformations.Move2_3, "3,5")]
    [InlineData(Transformations.RotateLeft90, "2,-1")]
    public async Task MovePlant(string transformationStr, string expectedCarrotLocationStr)
    {
        Transformation transformation = transformationStr;
        Location expectedCarrotLocation = expectedCarrotLocationStr;

        var command = new MovePlantCommand(
            GardenTestData.PeterGarden.Id,
            GardenBedTestData.PeterGardenBedWithCarrots.Id,
            GardenBedTestData.PeterGardenBedWithCarrots.Plants.First().Id,
            transformation);

        await ExecuteCommandAsync(command);

        var gardenBed = await GetAsync<GardenBed, GardenBedId>(GardenBedTestData.PeterGardenBedWithCarrots.Id);

        gardenBed!.Plants.First().Location.Should().BeEquivalentTo(expectedCarrotLocation);
    }
}
