namespace MyGardenPatch.Tests.Gardens.Commands;

public class DescribeGardenCommandTests : TestBase
{
    public DescribeGardenCommandTests()
    {
        SeedWith(UserTestData.PeterParker);
        SeedWith(GardenTestData.PeterGarden);

        SetCurrentUser(UserTestData.PeterParker.Id);
    }

    [Fact]
    public async Task DescribeGarden()
    {
        var command = new DescribeGardenCommand(
            GardenTestData.PeterGarden.Id,
            "New name",
            "A new description",
            new Uri("https://imagecdn/anewimage.jpg"),
            "A new image description");

        await ExecuteCommandAsync(command);

        var garden = await GetAsync<Garden, GardenId>(GardenTestData.PeterGarden.Id);

        garden!.Name.Should().Be(command.Name);
        garden!.Description.Should().Be(command.Description);
        garden!.ImageUri.Should().Be(command.ImageUri);
        garden!.ImageDescription.Should().Be(command.ImageDescription);
    }

    [Theory]
    [InlineData(GardenTestData.UnknownGardenId, "name", "description", "http://cdn/image.jpg", "image description", "Garden does not exist", "GardenId")]
    [InlineData(GardenTestData.PeterGardenId, null, "description", "http://cdn/image.jpg", "image description", "Name is required", "Name")]
    [InlineData(GardenTestData.PeterGardenId, "", "description", "http://cdn/image.jpg", "image description", "Name is required", "Name")]
    [InlineData(GardenTestData.PeterGardenId, Strings.Long201, "description", "http://cdn/image.jpg", "image description", "Not more than 200 characters", "Name")]
    [InlineData(GardenTestData.PeterGardenId, "name", "description", Strings.LongUri201, "image description", "Not more than 200 characters", "ImageUri")]
    public async Task InvalidCommand(Guid gardenId, string name, string description, string imageUri, string imageDescription, string expectedErrorMesssage, string expectedErrorPropertyPath)
    {
        var command = new DescribeGardenCommand(
            gardenId,
            name,
            description,
            new Uri(imageUri),
            imageDescription);

        Func<Task> task = () => ExecuteCommandAsync(command);

        await task.Should()
            .ThrowAsync<InvalidCommandException<DescribeGardenCommand>>()
            .WhereHasError(expectedErrorMesssage, expectedErrorPropertyPath);
    }
}
