namespace MyGardenPatch.Tests.GardenBeds.Commands;

public class RemovePlantCommandTests : TestBase
{
    public RemovePlantCommandTests()
    {
        SeedWith(UserTestData.PeterParker);
        SeedWith(GardenTestData.PeterGarden);
        SeedWith(GardenBedTestData.PeterGardenBedWithCarrots);
        SetCurrentUser(UserTestData.PeterParker.Id);
    }

    [Fact]
    public async Task RemovePlant()
    {
        var command = new RemovePlantCommand(
            GardenTestData.PeterGarden.Id,
            GardenBedTestData.PeterGardenBedWithCarrots.Id,
            GardenBedTestData.PeterGardenBedWithCarrots.Plants.First(p => p.Name == "Carrots").Id);

        await ExecuteCommandAsync(command);

        var gardenBed = await GetAsync<GardenBed, GardenBedId>(GardenBedTestData.PeterGardenBedWithCarrots.Id);

        gardenBed!.Plants.Should().NotContain(p => p.Name == "Carrots");

        MockDomainEventBus.Verify(
            bus => bus.PublishAsync(
                It.Is<PlantRemoved>(e => e.GardenId == GardenTestData.PeterGarden.Id &&
                                         e.GardenBedId == GardenBedTestData.PeterGardenBedWithCarrots.Id &&
                                         e.PlantId == GardenBedTestData.PeterGardenBedWithCarrots.Plants.First(p => p.Name == "Carrots").Id),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData(GardenTestData.UnknownGardenId, GardenBedTestData.PeterGardenBedWithCarrotsId, GardenBedTestData.CarrotId, "Garden does not exist", "GardenId")]
    [InlineData(GardenTestData.PeterGardenId, GardenBedTestData.UnknownGardenBedId, GardenBedTestData.CarrotId, "Garden bed does not exist", "GardenBedId")]
    [InlineData(GardenTestData.PeterGardenId, GardenBedTestData.PeterGardenBedWithCarrotsId, GardenBedTestData.UnknownPlantId, "Plant does not exist", "PlantId")]
    public async Task InvalidCommand(
        Guid gardenId,
        Guid gardenBedId,
        Guid plantId,
        string expectedErrorMessage,
        string expectedErrorPropertyPath)
    {
        var command = new RemovePlantCommand(
            gardenId,
            gardenBedId,
            plantId);

        Func<Task> task = () => ExecuteCommandAsync(command);

        await task.Should()
            .ThrowAsync<InvalidCommandException<RemovePlantCommand>>()
            .WhereHasError(expectedErrorMessage, expectedErrorPropertyPath);
    }
}
