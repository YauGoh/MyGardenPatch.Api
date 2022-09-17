namespace MyGardenPatch.Tests.Gardens.Commands;

public class StartNewGardenCommandTests : TestBase
{

    [Fact]
    public async Task StartsNewGarden()
    {
        SetCurrentUser(UserTestData.PeterParker.Id);
        SeedWith(UserTestData.PeterParker);

        var createdAt = new DateTime(2022, 1, 1);
        MockDateTimeProvider
            .Setup(_ => _.Now)
            .Returns(createdAt);

        await ExecuteCommandAsync(
            new StartNewGardenCommand(
                new GardenId(new Guid("{4DC2CEEF-063A-4D33-BFD7-D8595DA0A092}")),
                "A New Garden", 
                "Going to grow lots of potatos", 
                new Location(
                    LocationType.Point, 
                    new[] 
                    { 
                        new Point(20.0, 30.0) 
                    }), 
                new Uri("https://cdn/image.jpg"), 
                ""));

        var garden = await GetAsync<Garden, GardenId>(g => g.Name == "A New Garden");

        garden!.Id.Should().Be(new GardenId(new Guid("{4DC2CEEF-063A-4D33-BFD7-D8595DA0A092}")));
        garden.Name.Should().Be("A New Garden");
        garden.Description.Should().Be("Going to grow lots of potatos");
        garden.Location.Should().BeEquivalentTo(new Location(LocationType.Point, new[] { new Point(20.0, 30.0) }));
        garden.CreatedAt.Should().Be(createdAt);
        garden.GardenerId.Should().Be(UserTestData.PeterParker.Id);
    }

    [Theory]
    [InlineData("4DC2CEEF-063A-4D33-BFD7-D8595DA0A092", "", "Going to grow lots of potatos", 20, 30, "2022/1/1", "Name is required", "Name")]
    [InlineData("4DC2CEEF-063A-4D33-BFD7-D8595DA0A092", Strings.Long201, "Going to grow lots of potatos", 20, 30, "2022/1/1", "Not more than 200 characters", "Name")]
    [InlineData("00000000-0000-0000-0000-000000000000", "Front Garden", "Going to grow lots of potatos", 20, 30, "2022/1/1", "A valid garden id is required", "GardenId")]
    public async Task ShouldErrorForInvalidCommand(
        Guid gardenId,
        string name,
        string description,
        double lat,
        double lng,
        DateTime createdAt,
        string expectedErrorMessage,
        string expectedErrorPropertyPath,
        string because = "")
    {
        SetCurrentUser(UserTestData.PeterParker.Id);
        SeedWith(UserTestData.PeterParker);

        MockDateTimeProvider
            .Setup(_ => _.Now)
            .Returns(createdAt);

        Func<Task> task = () => ExecuteCommandAsync(new StartNewGardenCommand(gardenId, name, description, new Location(lat, lng), new Uri("https://cdn/image.jpg"), ""));

        await task.Should()
            .ThrowAsync<InvalidCommandException<StartNewGardenCommand>>()
            .WhereHasError(expectedErrorMessage, expectedErrorPropertyPath, because);
    }
}