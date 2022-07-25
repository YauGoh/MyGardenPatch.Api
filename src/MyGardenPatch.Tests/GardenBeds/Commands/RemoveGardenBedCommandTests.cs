namespace MyGardenPatch.Tests.GardenBeds.Commands;

public class RemoveGardenBedCommandTests : TestBase
{
    public RemoveGardenBedCommandTests()
    {
        SeedWith(UserTestData.PeterParker);
        SeedWith(GardenTestData.PeterGarden);
        SeedWith(GardenBedTestData.PeterFrontGardenBed);
        SetCurrentUser(UserTestData.PeterParker.Id);
    }

    [Fact]
    public async Task RemoveGardenBed()
    {
        var command = new RemoveGardenBedCommand(GardenTestData.PeterGarden.Id, GardenBedTestData.PeterFrontGardenBed.Id);

        await ExecuteCommandAsync(command);

        var exists = await AnyAsync<GardenBed, GardenBedId>(gb => gb.Id == GardenBedTestData.PeterFrontGardenBed.Id);

        exists.Should().BeFalse();

        MockDomainEventBus.Verify(
            bus => bus.PublishAsync<GardenBedRemoved>(
                It.Is<GardenBedRemoved>(e => e.GardenId == GardenTestData.PeterGarden.Id &&
                                             e.GardenBedId == GardenBedTestData.PeterFrontGardenBed.Id),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData(GardenTestData.UnknownGardenId, GardenBedTestData.PeterFrontGardenBedId, "Garden does not exist", "GardenId")]
    [InlineData(GardenTestData.PeterGardenId, GardenBedTestData.UnknownGardenBedId, "Garden bed does not exist", "GardenBedId")]
    public async Task InvalidCommand(
        Guid gardenId,
        Guid gardenBedId,
        string expectedErrorMessage,
        string expectedErrorPropertyPath)
    {
        var command = new RemoveGardenBedCommand(
           gardenId,
           gardenBedId);

        Func<Task> task = () => ExecuteCommandAsync(command);

        await task.Should()
            .ThrowAsync<InvalidCommandException<RemoveGardenBedCommand>>()
            .WhereHasError(expectedErrorMessage, expectedErrorPropertyPath);
    }
}
