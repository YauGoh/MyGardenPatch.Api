using MyGardenPatch.GardenBeds.Queries;

namespace MyGardenPatch.WebApi.Tests;

[TestCaseOrderer("MyGardenPatch.WebApi.Tests.OrderedByDependantTests", "MyGardenPatch.WebApi.Tests")]
public class CreateNewGarden : IClassFixture<TestFixture>
{
    private readonly TestFixture _fixture;

    public CreateNewGarden(TestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact, Order(0)]
    public async Task T000_SetUp()
    {
        await _fixture
            .WithRegisteredUser("Peter Parker", "peter.parker@email.com", "PASSWORD")
            .Setup();
    }

    [Fact, Order(1)]
    public async Task T001_StartNewGarden()
    {
        await _fixture
            .Command(
                "/commands/StartNewGardenCommand",
                new
                {
                    Name = "My first garden",
                    Description = "This is my first garden",
                    Location = new
                    {
                        Type = 1,
                        Points = new[]
                        {
                            new { X = 0, Y = 0 },
                            new { X = 1, Y = 0 },
                            new { X = 1, Y = 1 },
                            new { X = 0, Y = 1 },
                        }
                    }
                });
    }

    [Fact, Order(2)]
    public async Task T002_GetAllGardens()
    {
        var gardens = await _fixture
            .Query<IEnumerable<GardenDescription>>(
                "/queries/GetAllGardenDescriptionsQuery");

        gardens
            .Should()
            .SatisfyRespectively(
                first =>
                {
                    _fixture.WithGardenId(first.GardenId);

                    first.GardenId.Should().NotBeNull();
                    first.GardenId.Value.Should().NotBeEmpty();
                    first.Name.Should().Be("My first garden");
                    first.Description.Should().Be("This is my first garden");
                    first.Center.Should().Be(new Point(0.5, 0.5));
                    first.ImageUri.Should().BeNull();
                    first.ImageDescription.Should().BeNull();
                });
    }

    [Fact, Order(3)]
    public async Task T003_DescribeGarden()
    {
        await _fixture
            .Scenario(
                _ =>
                {
                    _.Post
                        .Json(new 
                        {
                            GardenId = _fixture.GardenId,
                            Name = "Front",
                            Description = "Planting carrots here",
                            ImageUri = "https://cdn/images/image.jpg",
                            ImageDescription = "Nothing here yet"
                        })
                        .ToUrl("/commands/DescribeGardenCommand");

                    _.StatusCodeShouldBeOk();
                });
    }

    [Fact, Order(4)]
    public async Task T004_GetAllGardens_AfterDescribe()
    {
        var gardens = await _fixture
            .Query<IEnumerable<GardenDescription>>(
                "/queries/GetAllGardenDescriptionsQuery");

        gardens
            .Should()
            .SatisfyRespectively(
                first =>
                {
                    first.GardenId.Should().Be(_fixture.GardenId);
                    first.Name.Should().Be("Front");
                    first.Description.Should().Be("Planting carrots here");
                    first.ImageUri.Should().BeEquivalentTo(new Uri("https://cdn/images/image.jpg"));
                    first.ImageDescription.Should().Be("Nothing here yet");
                });
    }

    [Fact, Order(5)]
    public async Task T005_AddGardenBed()
    {
        await _fixture
            .Command(
                "/commands/AddGardenBedCommand",
                new
                {
                    GardenId = _fixture.GardenId.Value,
                    Name = "Front",
                    Description = "infront of master bedroom window",
                    Location = new
                    {
                        Type = 1,
                        Points = new[]
                        {
                            new { X = 0, Y = 0 },
                            new { X = 1, Y = 0 },
                            new { X = 1, Y = 1 },
                            new { X = 0, Y = 1 },
                        }
                    }
                });
    }

    [Fact, Order(6)]
    public async Task T006_GetAllGardenBeds()
    {
        var gardenBeds =
            await _fixture
                .Query<IEnumerable<GardenBedDescription>>(
                    "/queries/GetAllGardenBedDescriptionsQuery",
                    new { GardenId = _fixture.GardenId });

        gardenBeds
            .Should()
            .SatisfyRespectively(
                first =>
                {
                    _fixture.WithGardenBedId(first.GardenBedId);

                    first.GardenBedId.Should().NotBeNull();
                    first.GardenBedId.Value.Should().NotBeEmpty();
                    first.Name.Should().Be("Front");
                    first.Description.Should().Be("infront of master bedroom window");
                    first.Center.Should().BeEquivalentTo(new { X = 0.5, Y = 0.5 });
                    first.ImageUri.Should().BeNull();
                    first.ImageDescription.Should().BeNull();
                });
    }

    [Fact, Order(7)]
    public async Task T007_DescribeGardenBed()
    {
        await _fixture
            .Command(
                "/commands/DescribeGardenBedCommand",
                new 
                { 
                    GardenId = _fixture.GardenId,
                    GardenBedId = _fixture.GardenBedId.Value,
                    Name = "Carrots",
                    Description = "Starting with dutch carrots",
                    ImageUri = "https://cdn/images.jpg",
                    ImageDescription = "Just seeds"
                });
    }

    [Fact, Order(8)]
    public async Task T008_GetAllGardenBeds_AfterDescribe()
    {
        var gardenBeds =
            await _fixture
                .Query<IEnumerable<GardenBedDescription>>(
                    "/queries/GetAllGardenBedDescriptionsQuery",
                    new { GardenId = _fixture.GardenId });

        gardenBeds
            .Should()
            .SatisfyRespectively(
                first =>
                {
                    first.Name.Should().Be("Carrots");
                    first.Description.Should().Be("Starting with dutch carrots");
                    first.ImageUri.Should().BeEquivalentTo(new Uri("https://cdn/images.jpg"));
                    first.ImageDescription.Should().Be("Just seeds");
                });
    }

    [Fact, Order(9)]
    public async Task T009_AddPlant()
    {
        await _fixture
            .Command(
                "/commands/AddPlantCommand",
                new
                {
                    GardenId = _fixture.GardenId.Value,
                    GardenBedId = _fixture.GardenBedId.Value,
                    Name = "Dutch Carrots",
                    Description = "Seedling purchased from bunnings",
                    Location = new
                    {
                        Type = 0,
                        Points = new[] 
                        {
                            new { X = 0, Y = 0 }
                        }
                    }
                });
    }
}
