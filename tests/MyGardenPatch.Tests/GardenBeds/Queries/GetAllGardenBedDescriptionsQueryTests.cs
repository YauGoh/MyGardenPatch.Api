﻿namespace MyGardenPatch.Tests.GardenBeds.Queries;

public class GetAllGardenBedDescriptionsQueryTests : TestBase
{
    public GetAllGardenBedDescriptionsQueryTests()
    {
        SeedWith(UserTestData.PeterParker);
        SeedWith(GardenTestData.PeterGarden);
        SeedWith(GardenBedTestData.PeterFrontGardenBed);
        SetCurrentUser(UserTestData.PeterParker);
    }

    [Fact]
    public async Task GetAllGardenBedDescriptions()
    {
        var query = new GetAllGardenBedDescriptionsQuery(GardenTestData.PeterGarden.Id);

        var results = await ExecuteQueryAsync(query);

        results
            .Should()
            .SatisfyRespectively(
                first =>
                {
                    first.GardenBedId.Should().Be(GardenBedTestData.PeterFrontGardenBed.Id);
                    first.Name.Should().Be(GardenBedTestData.PeterFrontGardenBed.Name);
                    first.Description.Should().Be(GardenBedTestData.PeterFrontGardenBed.Description);
                    first.Center.Should().Be(GardenBedTestData.PeterFrontGardenBed.Shape.Point);
                    first.Shape.Should().BeEquivalentTo(GardenBedTestData.PeterFrontGardenBed.Shape);
                    first.ImageUri.Should().Be(GardenBedTestData.PeterFrontGardenBed.ImageUri);
                    first.ImageDescription.Should().Be(GardenBedTestData.PeterFrontGardenBed.ImageDescription);
                });
    }

    [Fact]
    public async Task UnregisteredUser()
    {
        SetCurrentUser((GardenerId?)null);

        var query = new GetAllGardenBedDescriptionsQuery(GardenTestData.PeterGarden.Id);

        Func<Task> task = () => ExecuteQueryAsync(query);

        await task.Should()
            .ThrowAsync<InvalidQueryException<GetAllGardenBedDescriptionsQuery>>()
            .WhereHasError(
                "A registered logged in user is required",
                "");
    }
}
