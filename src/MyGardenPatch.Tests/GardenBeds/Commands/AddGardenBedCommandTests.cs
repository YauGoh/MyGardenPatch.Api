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
            new GardenBedId(),
            GardenTestData.PeterGarden.Id,
            "Carrots",
            "Growing Carrots here",
            Shapes.Rectangle_1x25,
            new Uri("https://cdn/images.jpg"),
            "A place");

        MockDateTimeProvider
            .Setup(dt => dt.Now)
            .Returns(new DateTime(2022, 1, 1));

        await ExecuteCommandAsync(command);

        var gardenBed = await GetAsync<GardenBed, GardenBedId>(g => g.Name == "Carrots");

        gardenBed!.Description.Should().Be("Growing Carrots here");
        gardenBed.Shape.Should().BeEquivalentTo(Shapes.Rectangle_1x25);
        gardenBed.ImageUri.Should().BeEquivalentTo(new Uri("https://cdn/images.jpg"));
        gardenBed.ImageDescription.Should().Be("A place");
        gardenBed.CreatedAt.Should().Be(new DateTime(2022, 1, 1));

        gardenBed.GardenerId.Should().Be(UserTestData.PeterParker.Id);
    }

    [Theory]
    [InlineData(GardenTestData.UnknownGardenId, "Carrots", "Growing Carrots here", "R 0, 0 0 1, 1", "http://cdn/images", "A place", "2022-01-01", "Garden does not exist", "GardenId")]
    [InlineData(GardenTestData.PeterGardenId, Strings.Long201, "Growing Carrots here", "R 0, 0 0 1, 1", "http://cdn/images", "A place", "2022-01-01", "Not more than 200 characters", "Name")]
    [InlineData(GardenTestData.PeterGardenId, "", "Growing Carrots here", "R 0, 0 0 1, 1", "http://cdn/images", "A place", "2022-01-01", "Name is required", "Name")]
    [InlineData(GardenTestData.PeterGardenId, null, "Growing Carrots here", "R 0, 0 0 1, 1", "http://cdn/images", "A place", "2022-01-01", "Name is required", "Name")]
    [InlineData(GardenTestData.PeterGardenId, "Carrots", "Growing Carrots here", null, "http://cdn/images", "A place", "2022-01-01", "A shape is required", "Shape")]
    public async Task InvalidCommand(
        Guid gardenId,
        string name,
        string description,
        string? shapeStr,
        string imageUri,
        string imageDescription,
        DateTime createdAt,
        string expectedErrorMessage,
        string expectedErrorPropertyPath)
    {
        MockDateTimeProvider
            .Setup(dt => dt.Now)
            .Returns(createdAt);

        var shape = (Shape?)shapeStr;

        var command = new AddGardenBedCommand(
            new GardenBedId(),
            gardenId,
            name,
            description,
            shape,
            new Uri(imageUri),
            imageDescription);

        Func<Task> task = () => ExecuteCommandAsync(command);

        await task.Should()
            .ThrowAsync<InvalidCommandException<AddGardenBedCommand>>()
            .WhereHasError(expectedErrorMessage, expectedErrorPropertyPath);
    }
}
