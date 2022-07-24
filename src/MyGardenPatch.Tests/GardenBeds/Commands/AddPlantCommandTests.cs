namespace MyGardenPatch.Tests.GardenBeds.Commands;

public class AddPlantCommandTest : TestBase
{
    public AddPlantCommandTest()
    {
        SeedWith(UserTestData.PeterParker);
        SeedWith(GardenTestData.PeterGarden);
        SeedWith(GardenBedTestData.PeterFrontGardenBed);
        SetCurrentUser(UserTestData.PeterParker.Id);
    }

    [Fact]
    public async Task AddPlant()
    {
        var command = new AddPlantCommand(
            GardenTestData.PeterGarden.Id,
            GardenBedTestData.PeterFrontGardenBed.Id,
            "Carrots",
            "Dutch",
            new Location(LocationType.Point, new[] { new Point(0, 0) }),
            new Uri("https://cdn/image.jpg"),
            "");

        MockDateTimeProvider
            .Setup(dt => dt.Now)
            .Returns(new DateTime(2022, 1, 1));

        await ExecuteCommandAsync(command);

        var gardenBed = await GetAsync<GardenBed, GardenBedId>(GardenBedTestData.PeterFrontGardenBed.Id);

        var plant = gardenBed!.Plants.First(p => p.Name == "Carrots");

        plant.Name.Should().Be(command.Name);
        plant.Description.Should().Be(command.Description);
        plant.Location.Should().BeEquivalentTo(command.Location);
        plant.ImageUri.Should().BeEquivalentTo(command.ImageUri);
        plant.ImageDescription.Should().BeEquivalentTo(command.ImageDescription);
        plant.CreatedAt.Should().Be(new DateTime(2022, 1, 1));

        MockDomainEventBus.Verify(
            bus => bus.PublishAsync<PlantAdded>(
                It.Is<PlantAdded>(
                    @event => @event.GardenId == GardenTestData.PeterGarden.Id &&
                              @event.GardenBedId == GardenBedTestData.PeterFrontGardenBed.Id &&
                              @event.PlantId == plant.Id &&
                              @event.PlantedAt == plant.CreatedAt),
                It.IsAny<CancellationToken>()),
            Times.Once);


    }

    [Theory]
    [InlineData(GardenTestData.UnknownGardenId, GardenBedTestData.PeterFrontGardenBedId, "Carrots", "Dutch", "https://cdn/images.jpg", "", "2022/01/01", "Garden does not exist", "GardenId", "0,0")]
    [InlineData(GardenTestData.PeterGardenId, GardenBedTestData.UnknownGardenBedId, "Carrots", "Dutch", "https://cdn/images.jpg", "", "2022/01/01", "Garden bed does not exist", "GardenBedId", "0,0")]
    [InlineData(GardenTestData.PeterGardenId, GardenBedTestData.PeterFrontGardenBedId, Strings.Long201, "Dutch", "https://cdn/images.jpg", "", "2022/01/01", "Not more than 200 characters", "Name", "0,0")]

    [InlineData(GardenTestData.PeterGardenId, GardenBedTestData.PeterFrontGardenBedId, "", "Dutch", "https://cdn/images.jpg", "", "2022/01/01", "Name is required", "Name", "0,0")]
    [InlineData(GardenTestData.PeterGardenId, GardenBedTestData.PeterFrontGardenBedId, null, "Dutch", "https://cdn/images.jpg", "", "2022/01/01", "Name is required", "Name", "0,0")]

    [InlineData(GardenTestData.PeterGardenId, GardenBedTestData.PeterFrontGardenBedId, "Carrots", "Dutch", "https://cdn/images.jpg", "", "2022/01/01", "A valid point or boundary is required", "Location")]
    [InlineData(GardenTestData.PeterGardenId, GardenBedTestData.PeterFrontGardenBedId, "Carrots", "Dutch", Strings.LongUri201, "", "2022/01/01", "Not more than 200 characters", "ImageUri", "0,0")]
    public async Task InvalidCommand(
        Guid gardenId,
        Guid gardenBedId,
        string name,
        string description,
        string imageUri,
        string imageDescription,
        DateTime createdAt,
        string expectedErrorMessage,
        string expectedErrorPropertyPath,
        params string[] points)
    {
        var command = new AddPlantCommand(
            gardenId,
            gardenBedId,
            name,
            description,
            new Location(LocationType.Point, points.Select(p => (Point)p).ToArray()),
            new Uri(imageUri, UriKind.Absolute),
            imageDescription);

        MockDateTimeProvider
            .Setup(dt => dt.Now)
            .Returns(createdAt);

        Func<Task> task = () => ExecuteCommandAsync(command);

        await task.Should()
            .ThrowAsync<InvalidCommandException<AddPlantCommand>>()
            .WhereHasError(expectedErrorMessage, expectedErrorPropertyPath);
    }
}