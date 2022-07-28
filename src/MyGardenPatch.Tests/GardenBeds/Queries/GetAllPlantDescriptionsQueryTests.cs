namespace MyGardenPatch.Tests.GardenBeds.Queries;

public class GetAllPlantDescriptionsQueryTests : TestBase
{
    public GetAllPlantDescriptionsQueryTests()
    {
        SeedWith(UserTestData.PeterParker);
        SeedWith(GardenTestData.PeterGarden);
        SeedWith(GardenBedTestData.PeterGardenBedWithCarrots);
        SetCurrentUser(UserTestData.PeterParker);
    }

    [Fact]
    public async Task GetAllPlantDescriptions()
    {
        var query = new GetAllPlantDescriptionsQuery(GardenTestData.PeterGarden.Id, GardenBedTestData.PeterGardenBedWithCarrots.Id);

        var results = await ExecuteQueryAsync(query);

        results
            .Should()
            .SatisfyRespectively(
                first =>
                {
                    var plant = GardenBedTestData.PeterGardenBedWithCarrots.Plants.First();

                    first.PlantId.Should().Be(plant.Id);
                    first.Name.Should().Be(plant.Name);
                    first.Description.Should().Be(plant.Description);
                    first.Center.Should().Be(plant.Location.Center);
                    first.Location.Should().BeEquivalentTo(plant.Location);
                    first.ImageUri.Should().Be(plant.ImageUri);
                    first.ImageDescription.Should().Be(plant.ImageDescription);
                });
    }

    [Fact]
    public async Task UnregisteredUser()
    {
        SetCurrentUser((UserId?)null);

        var query = new GetAllGardenBedDescriptionsQuery(GardenTestData.PeterGarden.Id);

        Func<Task> task = () => ExecuteQueryAsync(query);

        await task.Should()
            .ThrowAsync<InvalidQueryException<GetAllGardenBedDescriptionsQuery>>()
            .WhereHasError(
                "A registered logged in user is required",
                "");
    }
}
