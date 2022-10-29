namespace MyGardenPatch.Tests.Gardens.Queries;

public class GetGardenMapQueryTests : TestBase
{
    public GetGardenMapQueryTests()
    {
        SeedWith(UserTestData.PeterParker);
        SeedWith(GardenTestData.PeterGarden);
        SeedWith(GardenBedTestData.PeterGardenBedWithCarrots);

        SetCurrentUser(UserTestData.PeterParker);
    }

    [Fact]
    public async Task GetGardenMap()
    {
        var query = new GetGardenMapQuery(GardenTestData.PeterGarden.Id);

        var map = await ExecuteQueryAsync(query);

        map!.GardenId.Should().Be(GardenTestData.PeterGarden.Id);
        map.GardenerId.Should().Be(GardenTestData.PeterGarden.GardenerId);
        map.Name.Should().Be(GardenTestData.PeterGarden.Name);
        map.Description.Should().Be(GardenTestData.PeterGarden.Description);
        map.Center.Should().Be(GardenTestData.PeterGarden.Point);
        map.ImageUri.Should().Be(GardenTestData.PeterGarden.ImageUri);
        map.ImageDescription.Should().Be(GardenTestData.PeterGarden.ImageDescription);

        map.GardenBeds.Should().SatisfyRespectively(
            first =>
            {
                first.GardenBedId.Should().Be(GardenBedTestData.PeterGardenBedWithCarrots.Id);
                first.Name.Should().Be(GardenBedTestData.PeterGardenBedWithCarrots.Name);
                first.Description.Should().Be(GardenBedTestData.PeterGardenBedWithCarrots.Description);
                first.Center.Should().Be(GardenBedTestData.PeterGardenBedWithCarrots.Shape.Point);
                first.Shape.Should().Be(GardenBedTestData.PeterGardenBedWithCarrots.Shape);
                first.ImageUri.Should().Be(GardenBedTestData.PeterGardenBedWithCarrots.ImageUri);
                first.ImageDescription.Should().Be(GardenBedTestData.PeterGardenBedWithCarrots.ImageDescription);

                first.Plants.Should().SatisfyRespectively(
                    firstPlant =>
                    {
                        firstPlant.PlantId.Should().Be(GardenBedTestData.Carrots.Id);
                        firstPlant.Name.Should().Be(GardenBedTestData.Carrots.Name);
                        firstPlant.Description.Should().Be(GardenBedTestData.Carrots.Description);
                        firstPlant.Center.Should().Be(GardenBedTestData.Carrots.Shape.Point);
                        firstPlant.Shape.Should().Be(GardenBedTestData.Carrots.Shape);
                        firstPlant.ImageUri.Should().Be(GardenBedTestData.Carrots.ImageUri);
                        firstPlant.ImageDescription.Should().Be(GardenBedTestData.Carrots.ImageDescription);
                    });
            });
    }


}
