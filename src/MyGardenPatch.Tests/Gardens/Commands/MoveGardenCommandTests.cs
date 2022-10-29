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

    [Fact]
    public async Task MoveGardenBed()
    {
        Point newLocation = new Point(10, 4);

        var command = new MoveGardenCommand(
            GardenTestData.PeterGarden.Id,
            newLocation);

        await ExecuteCommandAsync(command);

        var garden = await GetAsync<Garden, GardenId>(GardenTestData.PeterGarden.Id);

        garden!.Center.Should().BeEquivalentTo(newLocation);
    }

    [Theory]
    [InlineData(GardenTestData.UnknownGardenId, Transformations.Identity, "Garden does not exist", "GardenId")]
    public async Task InvalidCommand(Guid gardenId, string transformationStr, string expectedErrorMessage, string expectedErrorPropertyPath)
    {
        Transformation transformation = transformationStr;

        var command = new MoveGardenCommand(
            gardenId,
            new Point(0, 1));

        Func<Task> task = () => ExecuteCommandAsync(command);

        await task.Should()
            .ThrowAsync<InvalidCommandException<MoveGardenCommand>>()
            .WhereHasError(expectedErrorMessage, expectedErrorPropertyPath);
    }
}
