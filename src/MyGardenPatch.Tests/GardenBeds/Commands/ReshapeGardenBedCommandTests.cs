namespace MyGardenPatch.Tests.GardenBeds.Commands;

public class ReshapeGardenBedCommandTests : TestBase
{
    public ReshapeGardenBedCommandTests()
    {
        SeedWith(UserTestData.PeterParker);
        SeedWith(GardenTestData.PeterGarden);
        SeedWith(GardenBedTestData.PeterGardenBedWithCarrots);
        SetCurrentUser(UserTestData.PeterParker.Id);
    }

    [Fact]
    public async Task ReshapeGardenBed()
    {

        var command = new ReshapeGardenBedCommand(
            GardenTestData.PeterGarden.Id, 
            GardenBedTestData.PeterGardenBedWithCarrots.Id, 
            Shapes.Circular_2);

        await ExecuteCommandAsync(command);

        var gardenBed = await GetAsync<GardenBed, GardenBedId>(GardenBedTestData.PeterGardenBedWithCarrots.Id);

        gardenBed!.Shape.Should().BeEquivalentTo(command.Shape);
    }

    [Theory]
    [InlineData(GardenTestData.UnknownGardenId, GardenBedTestData.PeterGardenBedWithCarrotsId, Shapes.Rectangle_1x25_str, "Garden does not exist", "GardenId")]
    [InlineData(GardenTestData.PeterGardenId, GardenBedTestData.UnknownGardenBedId, Shapes.Rectangle_1x25_str, "Garden bed does not exist", "GardenBedId")]
    public async Task InvalidCommand(Guid gardenId, Guid gardenBedId, string shapeStr, string expectedErrorMessage, string expectedErrorPropertyPath)
    {
        var shape = (Shape)shapeStr!;

        var command = new ReshapeGardenBedCommand(
            gardenId,
            gardenBedId,
            shape);

        Func<Task> task = () => ExecuteCommandAsync(command);

        await task.Should()
            .ThrowAsync<InvalidCommandException<ReshapeGardenBedCommand>>()
            .WhereHasError(expectedErrorMessage, expectedErrorPropertyPath);
    }
}
