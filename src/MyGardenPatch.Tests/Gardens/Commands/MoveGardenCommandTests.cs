using MyGardenPatch.Gardens.DomainEvents;

namespace MyGardenPatch.Tests.Gardens.Commands;

public class MoveGardenCommandTests : TestBase
{
    public MoveGardenCommandTests()
    {
        SeedWith(UserTestData.PeterParker);
        SeedWith(GardenTestData.PeterGarden);
        SetCurrentUser(UserTestData.PeterParker.Id);
    }

    [Theory]
    [InlineData(Transformations.Identity, "1,1")]
    [InlineData(Transformations.Move1x, "2,1")]
    [InlineData(Transformations.Move1y, "1,2")]
    [InlineData(Transformations.Move2_3, "3,4")]
    [InlineData(Transformations.RotateLeft90, "1,-1")]
    public async Task MoveGardenBed(string transformationStr, string expectedLocation)
    {
        Transformation transformation = transformationStr;
        Location expectedGardenBedLocation = expectedLocation;

        var command = new MoveGardenCommand(
            GardenTestData.PeterGarden.Id,
            transformation,
            true);

        await ExecuteCommandAsync(command);

        var garden = await GetAsync<Garden, GardenId>(GardenTestData.PeterGarden.Id);

        garden!.Location.Should().BeEquivalentTo(expectedGardenBedLocation);

        MockDomainEventBus.Verify(
            bus => bus.PublishAsync(
                It.Is<GardenMoved>(
                    e => e.GardenId == command.GardenId &&
                         e.MoveGardenBeds == command.MoveGardenBeds &&
                         e.Transformation == command.Transformation),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData(GardenTestData.UnknownGardenId, Transformations.Identity, "Garden does not exist", "GardenId")]
    public async Task InvalidCommand(Guid gardenId, string transformationStr, string expectedErrorMessage, string expectedErrorPropertyPath)
    {
        Transformation transformation = transformationStr;

        var command = new MoveGardenCommand(
            gardenId,
            transformation,
            true);

        Func<Task> task = () => ExecuteCommandAsync(command);

        await task.Should()
            .ThrowAsync<InvalidCommandException<MoveGardenCommand>>()
            .WhereHasError(expectedErrorMessage, expectedErrorPropertyPath);
    }
}
