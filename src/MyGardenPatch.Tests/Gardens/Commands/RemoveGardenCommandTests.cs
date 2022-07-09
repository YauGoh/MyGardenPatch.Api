namespace MyGardenPatch.Tests.Gardens.Commands;

public class RemoveGardenCommandTests : TestBase
{
    public RemoveGardenCommandTests()
    {
        SeedWith(UserTestData.PeterParker);
        SeedWith(GardenTestData.PeterGarden);
        SetCurrentUser(UserTestData.PeterParker.Id);
    }

    [Fact]
    public async Task RemoveGarden()
    {
        var command = new RemoveGardenCommand(GardenTestData.PeterGarden.Id);

        await ExecuteCommandAsync(command);

        var exists = await AnyAsync<Garden, GardenId>(g => g.Id == GardenTestData.PeterGarden.Id);

        exists.Should().BeFalse();

        MockDomainEventBus.Verify(
            bus => bus.PublishAsync<GardenRemoved>(
                It.Is<GardenRemoved>(e => e.GardenId == GardenTestData.PeterGarden.Id),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData(GardenTestData.UnknownGardenId, "Garden does not exist", "GardenId")]
    public async Task InvalidCommand(
        Guid gardenId,
        string expectedErrorMessage,
        string expectedErrorPropertyPath)
    {
        var command = new RemoveGardenCommand(
           gardenId);

        Func<Task> task = () => ExecuteCommandAsync(command);

        await task.Should()
            .ThrowAsync<InvalidCommandException<RemoveGardenCommand>>()
            .WhereHasError(expectedErrorMessage, expectedErrorPropertyPath);
    }
}
