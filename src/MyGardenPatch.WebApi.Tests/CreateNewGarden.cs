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
            .Query<IEnumerable<GardenDescriptor>>(
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
            .Query<IEnumerable<GardenDescriptor>>(
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
                .Query<IEnumerable<GardenBedDescriptor>>(
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
                .Query<IEnumerable<GardenBedDescriptor>>(
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
                    Name = "Carrots",
                    Description = "Seedling",
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

    [Fact, Order(10)]
    public async Task T010_GetAllPlants()
    {
        var plants =
            await _fixture
                .Query<IEnumerable<PlantDescription>>(
                    "/queries/GetAllPlantDescriptionsQuery",
                    new 
                    { 
                        GardenId = _fixture.GardenId,
                        GardenBedId = _fixture.GardenBedId,
                    });

        plants
            .Should()
            .SatisfyRespectively(
                first =>
                {
                    _fixture.WithPlantId(first.PlantId);

                    first.PlantId.Should().NotBeNull();
                    first.PlantId.Value.Should().NotBeEmpty();
                    first.Name.Should().Be("Carrots");
                    first.Description.Should().Be("Seedling");
                    first.Center.Should().BeEquivalentTo(new { X = 0, Y = 0 });
                    first.ImageUri.Should().BeNull();
                    first.ImageDescription.Should().BeNull();
                });
    }

    [Fact, Order(11)]
    public async Task T011_DescribePlant()
    {
        await _fixture
            .Command(
                "/commands/DescribePlantCommand",
                new
                {
                    GardenId = _fixture.GardenId.Value,
                    GardenBedId = _fixture.GardenBedId.Value,
                    PlantId = _fixture.PlantId.Value,
                    Name = "Dutch Carrots",
                    Description = "Seedling purchased from bunnings",
                    ImageUri = "https://cdn/images/plant.jpg",
                    ImageDescription = "Just planted"
                });
    }

    [Fact, Order(12)]
    public async Task T012_GetAllPlants_AfterDescribe()
    {
        var plants =
            await _fixture
                .Query<IEnumerable<PlantDescription>>(
                    "/queries/GetAllPlantDescriptionsQuery",
                    new
                    {
                        GardenId = _fixture.GardenId,
                        GardenBedId = _fixture.GardenBedId,
                    });

        plants
            .Should()
            .SatisfyRespectively(
                first =>
                {
                    _fixture.WithPlantId(first.PlantId);

                    first.Name.Should().Be("Dutch Carrots");
                    first.Description.Should().Be("Seedling purchased from bunnings");
                    first.ImageUri.Should().BeEquivalentTo(new Uri("https://cdn/images/plant.jpg"));
                    first.ImageDescription.Should().Be("Just planted");
                });
    }

    [Fact, Order(13)]
    public async Task T013_MoveGarden()
    {
        await _fixture
            .Command(
                "/commands/MoveGardenCommand",
                new
                {
                    GardenId = _fixture.GardenId.Value,
                    Transformation = @"1 0 0
                                       0 1 0
                                       1 0 1",
                    MoveGardenBeds = true
                });
    }

    [Fact, Order(14)]
    public async Task T014_GetGardens_AfterMove()
    {
        var gardens =
            await _fixture
                .Query<IEnumerable<GardenDescriptor>>(
                    "/queries/GetAllGardenDescriptionsQuery",
                    new { });

        gardens
            .Should()
            .SatisfyRespectively(
                first =>
                {
                    first.Center.Should().BeEquivalentTo(new { X = 1.5, Y = 0.5 });
                });
    }

    [Fact, Order(15)]
    public async Task T015_GetAllGardenBeds_AfterMove()
    {
        var gardenBeds =
            await _fixture
                .Query<IEnumerable<GardenBedDescriptor>>(
                    "/queries/GetAllGardenBedDescriptionsQuery",
                    new { GardenId = _fixture.GardenId.Value });

        gardenBeds
            .Should()
            .SatisfyRespectively(
                first =>
                {
                    first.Center.Should().BeEquivalentTo(new { X = 1.5, Y = 0.5 });
                });
    }

    [Fact, Order(16)]
    public async Task T016_GetAllPlants_AfterMove()
    {
        var plants =
            await _fixture
                .Query<IEnumerable<PlantDescription>>(
                    "/queries/GetAllPlantDescriptionsQuery",
                    new 
                    {
                        GardenId = _fixture.GardenId.Value,
                        GardenBedId = _fixture.GardenBedId.Value
                    });

        plants
            .Should()
            .SatisfyRespectively(
                first =>
                {
                    first.Center.Should().BeEquivalentTo(new { X = 1.0, Y = 0.0 });
                });
    }

    [Fact, Order(17)]
    public async Task T017_MoveGardenBed()
    {
        await _fixture
            .Command(
                "/commands/MoveGardenBedCommand",
                new
                {
                    GardenId = _fixture.GardenId.Value,
                    GardenBedId = _fixture.GardenBedId.Value,
                    Transformation = @"1 0 0
                                       0 1 0
                                       1 0 1"
                });
    }

    [Fact, Order(18)]
    public async Task T018_GetAllGardenBeds_AfterMoveGardenBed()
    {
        var gardenBeds =
            await _fixture
                .Query<IEnumerable<GardenBedDescriptor>>(
                    "/queries/GetAllGardenBedDescriptionsQuery",
                    new { GardenId = _fixture.GardenId.Value });

        gardenBeds
            .Should()
            .SatisfyRespectively(
                first =>
                {
                    first.Center.Should().BeEquivalentTo(new { X = 2.5, Y = 0.5 });
                });
    }

    [Fact, Order(19)]
    public async Task T019_GetAllPlants_AfterMoveGardenBed()
    {
        var plants =
            await _fixture
                .Query<IEnumerable<PlantDescription>>(
                    "/queries/GetAllPlantDescriptionsQuery",
                    new
                    {
                        GardenId = _fixture.GardenId.Value,
                        GardenBedId = _fixture.GardenBedId.Value
                    });

        plants
            .Should()
            .SatisfyRespectively(
                first =>
                {
                    first.Center.Should().BeEquivalentTo(new { X = 2.0, Y = 0.0 });
                });
    }

    [Fact, Order(20)]
    public async Task T020_MovePlant()
    {
        await _fixture
            .Command(
                "/commands/MovePlantCommand",
                new
                {
                    GardenId = _fixture.GardenId.Value,
                    GardenBedId = _fixture.GardenBedId.Value,
                    PlantId = _fixture.PlantId.Value,
                    Transformation = @"1 0 0
                                       0 1 0
                                       1 0 1"
                });
    }

    [Fact, Order(21)]
    public async Task T021_GetAllPlants_AfterMovePlant()
    {
        var plants =
            await _fixture
                .Query<IEnumerable<PlantDescription>>(
                    "/queries/GetAllPlantDescriptionsQuery",
                    new
                    {
                        GardenId = _fixture.GardenId.Value,
                        GardenBedId = _fixture.GardenBedId.Value
                    });

        plants
            .Should()
            .SatisfyRespectively(
                first =>
                {
                    first.Center.Should().BeEquivalentTo(new { X = 3.0, Y = 0.0 });
                });
    }

    [Fact, Order(22)]
    public async Task T022_RemovePlant()
    {
        await _fixture
            .Command(
                "/commands/RemovePlantCommand",
                new
                {
                    GardenId = _fixture.GardenId.Value,
                    GardenBedId = _fixture.GardenBedId.Value,
                    PlantId = _fixture.PlantId.Value
                });
    }

    [Fact, Order(23)]
    public async Task T023_GetAllPlants_AfterRemovePlant()
    {
        var plants =
            await _fixture
                .Query<IEnumerable<PlantDescription>>(
                    "/queries/GetAllPlantDescriptionsQuery",
                    new
                    {
                        GardenId = _fixture.GardenId.Value,
                        GardenBedId = _fixture.GardenBedId.Value
                    });

        plants
            .Should()
            .BeEmpty();
    }

    [Fact, Order(24)]
    public async Task T024_RemoveGardenBed()
    {
        await _fixture
            .Command(
                "/commands/RemoveGardenBedCommand",
                new
                {
                    GardenId = _fixture.GardenId.Value,
                    GardenBedId = _fixture.GardenBedId.Value
                });
    }

    [Fact, Order(25)]
    public async Task T025_GetAllGardenBeds_AfterRemoveGardenBed()
    {
        var gardenBeds =
            await _fixture
                .Query<IEnumerable<GardenBedDescriptor>>(
                    "/queries/GetAllGardenBedDescriptionsQuery",
                    new
                    {
                        GardenId = _fixture.GardenId.Value
                    });

        gardenBeds
            .Should()
            .BeEmpty();
    }

    [Fact, Order(26)]
    public async Task T026_RemoveGarden()
    {
        await _fixture
            .Command(
                "/commands/RemoveGardenCommand",
                new
                {
                    GardenId = _fixture.GardenId.Value,
                });
    }

    [Fact, Order(27)]
    public async Task T027_GetAllGardens_AfterRemoveGarden()
    {
        var gardens =
            await _fixture
                .Query<IEnumerable<GardenBedDescriptor>>(
                    "/queries/GetAllGardenDescriptionsQuery",
                    new { });

        gardens
            .Should()
            .BeEmpty();
    }
}
