namespace MyGardenPatch.Tests.GardenBeds.Commands;

public class MoveGardenBedCommandTests : TestBase
{
    public MoveGardenBedCommandTests()
    {
        SeedWith(UserTestData.PeterParker);
        SeedWith(GardenTestData.PeterGarden);
        SeedWith(GardenBedTestData.PeterGardenBedWithCarrots);
        SetCurrentUser(UserTestData.PeterParker.Id);
    }

    [Theory]
    [InlineData(Transformations.Identity, "1,1", "1,2")]
    [InlineData(Transformations.Move1x, "2,1", "2,2")]
    [InlineData(Transformations.Move1y, "1,2", "1,3")]
    [InlineData(Transformations.Move2_3, "3,4", "3,5")]
    [InlineData(Transformations.RotateLeft90, "1,-1", "2,-1")]
    public async Task MoveGardenBed(string transformationStr, string expectedGardenBedLocationStr, string expectedCarrotLocationStr)
    {
        Transformation transformation = transformationStr;
        Location expectedGardenBedLocation = expectedGardenBedLocationStr;
        Location expectedCarrotLocation = expectedCarrotLocationStr;

        var command = new MoveGardenBedCommand(GardenTestData.PeterGarden.Id, GardenBedTestData.PeterGardenBedWithCarrots.Id, transformation);

        await ExecuteCommandAsync(command);

        var gardenBed = await GetAsync<GardenBed, GardenBedId>(GardenBedTestData.PeterGardenBedWithCarrots.Id);

        gardenBed!.Location.Should().BeEquivalentTo(expectedGardenBedLocation);
        gardenBed!.Plants.First().Location.Should().BeEquivalentTo(expectedCarrotLocation);
    }

    [Theory]
    [InlineData(GardenTestData.UnknownGardenId, GardenBedTestData.PeterGardenBedWithCarrotsId, Transformations.Identity, "Garden does not exist", "GardenId")]
    [InlineData(GardenTestData.PeterGardenId, GardenBedTestData.UnknownGardenBedId, Transformations.Identity, "Garden bed does not exist", "GardenBedId")]
    public async Task InvalidCommand(Guid gardenId, Guid gardenBedId, string transformationStr, string expectedErrorMessage, string expectedErrorPropertyPath)
    {
        Transformation transformation = transformationStr;

        var command = new MoveGardenBedCommand(
            gardenId,
            gardenBedId,
            transformation);

        Func<Task> task = () => ExecuteCommandAsync(command);

        await task.Should()
            .ThrowAsync<InvalidCommandException<MoveGardenBedCommand>>()
            .WhereHasError(expectedErrorMessage, expectedErrorPropertyPath);
    }
}
