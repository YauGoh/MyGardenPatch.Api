namespace MyGardenPatch.Tests.Gardens.Queries;

public class GetAllGardenDescriptionsQueryTests : TestBase
{
    public GetAllGardenDescriptionsQueryTests()
    {
        SeedWith(UserTestData.PeterParker);
        SeedWith(GardenTestData.PeterGarden);
        SetCurrentUser(UserTestData.PeterParker);
    }

    [Fact]
    public async Task GetAllGardenDescriptions()
    {
        var results = await ExecuteQueryAsync(new GetAllGardenDescriptionsQuery());

        results
            .Should()
            .SatisfyRespectively(
                first =>
                {
                    first.GardenId.Should().Be(GardenTestData.PeterGarden.Id);
                    first.Name.Should().Be(GardenTestData.PeterGarden.Name);
                    first.Description.Should().Be(GardenTestData.PeterGarden.Description);
                    first.ImageUri.Should().Be(GardenTestData.PeterGarden.ImageUri);
                    first.ImageDescription.Should().Be(GardenTestData.PeterGarden.ImageDescription);
                    first.Center.Should().Be(GardenTestData.PeterGarden.Location.Center);
                    first.Location.Should().BeEquivalentTo(GardenTestData.PeterGarden.Location);
                });
    }

    [Fact]
    public async Task UnregisteredUser()
    {
        SetCurrentUser((GardenerId?)null);

        var query = new GetAllGardenDescriptionsQuery();

        Func<Task> task = () => ExecuteQueryAsync(query);

        await task.Should()
            .ThrowAsync<InvalidQueryException<GetAllGardenDescriptionsQuery>>()
            .WhereHasError(
                "A registered logged in user is required",
                "");
    }

}
