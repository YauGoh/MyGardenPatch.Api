namespace MyGardenPatch.Tests.GardenBeds.Commands;

public class DescribePlantCommandTests : TestBase
{
    public DescribePlantCommandTests()
    {
        SeedWith(UserTestData.PeterParker);
        SeedWith(GardenTestData.PeterGarden);
        SeedWith(GardenBedTestData.PeterGardenBedWithCarrots);
        SetCurrentUser(UserTestData.PeterParker.Id);
    }

    [Fact]
    public async Task DescribePlant()
    {
        var command = new DescribePlantCommand(
            GardenTestData.PeterGarden.Id,
            GardenBedTestData.PeterGardenBedWithCarrots.Id,
            GardenBedTestData.PeterGardenBedWithCarrots.Plants.First().Id,
            "New Name",
            "A new description",
            new Uri("https://image/newimage.jpg"),
            "A new image");

        await ExecuteCommandAsync(command);

        var gardenBed = await GetAsync<GardenBed, GardenBedId>(GardenBedTestData.PeterGardenBedWithCarrots.Id);

        var plant = gardenBed!.Plants.First();

        plant.Name.Should().Be(command.Name);
        plant.Description.Should().Be(command.Description);
        plant.ImageUri.Should().Be(command.ImageUri);
        plant.ImageDescription.Should().Be(command.ImageDescription);
    }

    [Theory]
    [InlineData(GardenTestData.UnknownGardenId, GardenBedTestData.PeterFrontGardenBedId, GardenBedTestData.CarrotId, "name", "description", "http://cdn/image.jpg", "image description", "Garden does not exist", "GardenId")]
    [InlineData(GardenTestData.PeterGardenId, GardenBedTestData.UnknownGardenBedId, GardenBedTestData.CarrotId, "name", "description", "http://cdn/image.jpg", "image description", "Garden bed does not exist", "GardenBedId")]
    [InlineData(GardenTestData.PeterGardenId, GardenBedTestData.PeterFrontGardenBedId, GardenBedTestData.UnknownPlantId, "name", "description", "http://cdn/image.jpg", "image description", "Plant does not exist", "PlantId")]
    [InlineData(GardenTestData.PeterGardenId, GardenBedTestData.PeterFrontGardenBedId, GardenBedTestData.CarrotId, "", "description", "http://cdn/image.jpg", "image description", "Name is required", "Name")]
    [InlineData(GardenTestData.PeterGardenId, GardenBedTestData.PeterFrontGardenBedId, GardenBedTestData.CarrotId, null, "description", "http://cdn/image.jpg", "image description", "Name is required", "Name")]
    [InlineData(GardenTestData.PeterGardenId, GardenBedTestData.PeterFrontGardenBedId, GardenBedTestData.CarrotId, Strings.Long201, "description", "http://cdn/image.jpg", "image description", "Not more than 200 characters", "Name")]
    [InlineData(GardenTestData.PeterGardenId, GardenBedTestData.PeterFrontGardenBedId, GardenBedTestData.CarrotId, "name", "description", Strings.LongUri201, "image description", "Not more than 200 characters", "ImageUri")]
    public async Task InvalidCommand(
        Guid gardenId,
        Guid gardenBedId,
        Guid plantId,
        string name,
        string description,
        string imageUri,
        string imageDescription,
        string expectedErrorMessage,
        string expectedErrorPropertyPath)
    {
        var command = new DescribePlantCommand(
            gardenId,
            gardenBedId,
            plantId,
            name,
            description,
            new Uri(imageUri),
            imageDescription);

        Func<Task> task = () => ExecuteCommandAsync(command);

        await task.Should()
            .ThrowAsync<InvalidCommandException<DescribePlantCommand>>()
            .WhereHasError(expectedErrorMessage, expectedErrorPropertyPath);
    }
}
