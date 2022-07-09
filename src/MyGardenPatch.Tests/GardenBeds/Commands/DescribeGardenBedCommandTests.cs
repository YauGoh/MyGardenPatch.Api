namespace MyGardenPatch.Tests.GardenBeds.Commands;

public class DescribeGardenBedCommandTests : TestBase
{
    public DescribeGardenBedCommandTests()
    {
        SeedWith(UserTestData.PeterParker);
        SeedWith(GardenTestData.PeterGarden);
        SeedWith(GardenBedTestData.PeterFrontGardenBed);
        SetCurrentUser(UserTestData.PeterParker.Id);
    }

    [Fact]
    public async Task DescribeGardenBed()
    {
        var command = new DescribeGardenBedCommand(
            GardenTestData.PeterGarden.Id,
            GardenBedTestData.PeterFrontGardenBed.Id,
            "New Name",
            "A new description",
            new Uri("https://image/newimage.jpg"),
            "A new image");

        await ExecuteCommandAsync(command);

        var gardenBed = await GetAsync<GardenBed, GardenBedId>(GardenBedTestData.PeterFrontGardenBed.Id);

        gardenBed!.Name.Should().Be(command.Name);
        gardenBed!.Description.Should().Be(command.Description);
        gardenBed!.ImageUri.Should().Be(command.ImageUri);
        gardenBed!.ImageDescription.Should().Be(command.ImageDescription);
    }

    [Theory]
    [InlineData(GardenTestData.UnknownGardenId, GardenBedTestData.PeterFrontGardenBedId, "name", "description", "http://cdn/image.jpg", "image description", "Garden does not exist", "GardenId")]
    [InlineData(GardenTestData.PeterGardenId, GardenBedTestData.UnknownGardenBedId, "name", "description", "http://cdn/image.jpg", "image description", "Garden bed does not exist", "GardenBedId")]
    [InlineData(GardenTestData.PeterGardenId, GardenBedTestData.PeterFrontGardenBedId, "", "description", "http://cdn/image.jpg", "image description", "Name is required", "Name")]
    [InlineData(GardenTestData.PeterGardenId, GardenBedTestData.PeterFrontGardenBedId, null, "description", "http://cdn/image.jpg", "image description", "Name is required", "Name")]
    [InlineData(GardenTestData.PeterGardenId, GardenBedTestData.PeterFrontGardenBedId, Strings.Long201, "description", "http://cdn/image.jpg", "image description", "Not more than 200 characters", "Name")]
    [InlineData(GardenTestData.PeterGardenId, GardenBedTestData.PeterFrontGardenBedId, "name", "description", Strings.LongUri201, "image description", "Not more than 200 characters", "ImageUri")]
    public async Task InvalidCommand(
        Guid gardenId,
        Guid gardenBedId,
        string name,
        string description,
        string imageUri,
        string imageDescription,
        string expectedErrorMessage,
        string expectedErrorPropertyPath)
    {
        var command = new DescribeGardenBedCommand(
            gardenId,
            gardenBedId,
            name,
            description,
            new Uri(imageUri),
            imageDescription);

        Func<Task> task = () => ExecuteCommandAsync(command);

        await task.Should()
            .ThrowAsync<InvalidCommandException<DescribeGardenBedCommand>>()
            .WhereHasError(expectedErrorMessage, expectedErrorPropertyPath);
    }
}