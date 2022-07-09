namespace MyGardenPatch.Tests.GardenBeds.Commands;

public class AddGardenBedCommandTests : TestBase
{
    public AddGardenBedCommandTests()
    {
        SeedWith(UserTestData.PeterParker);
        SeedWith(GardenTestData.PeterGarden);
        SetCurrentUser(UserTestData.PeterParker.Id);
    }

    [Fact]
    public async Task AddGardenBed()
    {
        var command = new AddGardenBedCommand(
            GardenTestData.PeterGarden.Id,
            "Carrots",
            "Growing Carrots here",
            new Location(LocationType.Point, new[] { new Point(0, 0) }),
            new Uri("https://cdn/images.jpg"),
            "A place",
            new DateTime(2022, 1, 1));

        await ExecuteCommandAsync(command);

        var gardenBed = await GetAsync<GardenBed, GardenBedId>(g => g.Name == "Carrots");

        gardenBed!.Description.Should().Be("Growing Carrots here");
        gardenBed.Location.Should().BeEquivalentTo(new Location(LocationType.Point, new[] { new Point(0, 0) }));
        gardenBed.ImageUri.ToString().Should().Be("https://cdn/images.jpg");
        gardenBed.ImageDescription.Should().Be("A place");
        gardenBed.CreatedAt.Should().Be(new DateTime(2022, 1, 1));

        gardenBed.UserId.Should().Be(UserTestData.PeterParker.Id);
    }

    [Theory]
    [InlineData(GardenTestData.UnknownGardenId, "Carrots", "Growing Carrots here", "http://cdn/images", "A place", "2022-01-01", "Garden does not exist", "GardenId", "Invalid garden id", "0,0")]

    [InlineData(GardenTestData.PeterGardenId, Strings.Long201, "Growing Carrots here", "http://cdn/images", "A place", "2022-01-01", "Not more than 200 characters", "Name", "Name too long", "0,0")]
    [InlineData(GardenTestData.PeterGardenId, "", "Growing Carrots here", "http://cdn/images", "A place", "2022-01-01", "Name is required", "Name", "Blank name", "0,0")]
    [InlineData(GardenTestData.PeterGardenId, null, "Growing Carrots here", "http://cdn/images", "A place", "2022-01-01", "Name is required", "Name", "Null name", "0,0")]

    [InlineData(GardenTestData.PeterGardenId, "Carrots", "Growing Carrots here", "http://cdn/images", "A place", "2022-01-01", "A valid point or boundary is required", "Location", "No location provided")]
    public async Task InvalidCommand(
        Guid gardenId,
        string name,
        string description,
        string imageUri,
        string imageDescription,
        DateTime createdAt,
        string expectedErrorMessage,
        string expectedErrorPropertyPath,
        string because,
        params string[] strPoints)
    {
        var points = strPoints
            .Select(str => (Point)str)
            .ToArray();

        var command = new AddGardenBedCommand(
            gardenId,
            name,
            description,
            new Location(LocationType.Point, points),
            new Uri(imageUri),
            imageDescription,
            createdAt);

        Func<Task> task = () => ExecuteCommandAsync(command);

        await task.Should()
            .ThrowAsync<InvalidCommandException<AddGardenBedCommand>>()
            .WhereHasError(expectedErrorMessage, expectedErrorPropertyPath, because);
    }
}
