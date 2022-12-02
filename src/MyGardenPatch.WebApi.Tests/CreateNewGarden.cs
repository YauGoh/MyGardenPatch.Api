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

    [Fact, Order(1.0)]
    public async Task T10000_StartNewGarden()
    {
        var gardenId = Guid.NewGuid();
        var imageId = Guid.NewGuid();

        await _fixture
            .Command(
                "/commands/StartNewGardenCommand",
                new
                {
                    GardenId = gardenId,
                    Name = "My first garden",
                    Description = "This is my first garden",
                    Center = new { X = 0.5, Y = 0.5 }
                },
                
                (
                    Name: $"gardenId={gardenId}&imageId={imageId}",
                    Filename: $"image.jpg",
                    ContentType: "image/jpeg",
                    Content: "Not really an image",
                    Headers: new Dictionary<string, string>
                    {
                        { "gardenId", gardenId.ToString() },
                        { "imageId", imageId.ToString() }
                    }
                ));


        _fixture.MockFileStorage
            .Verify(
                fs => fs.SaveAsync(
                    It.IsAny<GardenerId>(), 
                    It.Is<GardenId>(id => id.Value == gardenId), 
                    It.Is<ImageId>(id => id.Value == imageId), 
                    "image.jpg", 
                    "image/jpeg", 
                    It.IsAny<Stream>()),
                Times.Once);
    }

    [Fact, Order(2.0)]
    public async Task T20000_GetAllGardens()
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

    [Fact, Order(2.1)]
    public async Task T21000_DescribeGarden()
    {
        await _fixture
            .Scenario(
                _ =>
                {
                    _.Post
                        .Json(new 
                        {
                            GardenId = _fixture.GetGardenId(),
                            Name = "Front",
                            Description = "Planting carrots here",
                            ImageUri = "https://cdn/images/image.jpg",
                            ImageDescription = "Nothing here yet"
                        })
                        .ToUrl("/commands/DescribeGardenCommand");

                    _.StatusCodeShouldBeOk();
                });
    }

    [Fact, Order(2.2)]
    public async Task T22000_GetAllGardens_AfterDescribe()
    {
        var gardens = await _fixture
            .Query<IEnumerable<GardenDescriptor>>(
                "/queries/GetAllGardenDescriptionsQuery");

        gardens
            .Should()
            .SatisfyRespectively(
                first =>
                {
                    first.GardenId.Should().Be(_fixture.GetGardenId());
                    first.Name.Should().Be("Front");
                    first.Description.Should().Be("Planting carrots here");
                    first.ImageUri.Should().BeEquivalentTo(new Uri("https://cdn/images/image.jpg"));
                    first.ImageDescription.Should().Be("Nothing here yet");
                });
    }

    [Fact, Order(3.0)]
    public async Task T30000_AddGardenBed()
    {
        var gardenBedId = Guid.NewGuid();

        await _fixture
            .Command(
                "/commands/AddGardenBedCommand",
                new
                {
                    GardenId = _fixture.GetGardenId().Value,
                    GardenBedId = gardenBedId,
                    Name = "Front",
                    Description = "infront of master bedroom window",
                    Shape = new 
                    {
                        Type = ShapeType.Circular,
                        Point = new Point(0.5, 0.5),
                        Rotation = 0,
                        Radius = 2.0
                    }
                });
    }

    [Fact, Order(3.1)]
    public async Task T31000_GetAllGardenBeds()
    {
        var gardenBeds =
            await _fixture
                .Query<IEnumerable<GardenBedDescriptor>>(
                    "/queries/GetAllGardenBedDescriptionsQuery",
                    new { GardenId = _fixture.GetGardenId() });

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

    [Fact, Order(3.2)]
    public async Task T32000_DescribeGardenBed()
    {
        await _fixture
            .Command(
                "/commands/DescribeGardenBedCommand",
                new 
                { 
                    GardenId = _fixture.GetGardenId().Value,
                    GardenBedId = _fixture.GetGardenBedId().Value,
                    Name = "Carrots",
                    Description = "Starting with dutch carrots",
                    ImageUri = "https://cdn/images.jpg",
                    ImageDescription = "Just seeds"
                });
    }

    [Fact, Order(3.3)]
    public async Task T33000_GetAllGardenBeds_AfterDescribe()
    {
        var gardenBeds =
            await _fixture
                .Query<IEnumerable<GardenBedDescriptor>>(
                    "/queries/GetAllGardenBedDescriptionsQuery",
                    new { GardenId = _fixture.GetGardenId().Value });

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

    [Fact, Order(4.0)]
    public async Task T40000_AddPlant()
    {
        await _fixture
            .Command(
                "/commands/AddPlantCommand",
                new
                {
                    GardenId = _fixture.GetGardenId().Value,
                    GardenBedId = _fixture.GetGardenBedId().Value,
                    Name = "Carrots",
                    Description = "Seedling",
                    Shape = new 
                    {
                        Type = ShapeType.Rectangular,
                        Point = new { X = 0, Y = 0 },
                        Rotation = 0,
                        Width = 0.2,
                        Height = 0.2
                    }
                });
    }

    [Fact, Order(4.1)]
    public async Task T41000_GetAllPlants()
    {
        var plants =
            await _fixture
                .Query<IEnumerable<PlantDescription>>(
                    "/queries/GetAllPlantDescriptionsQuery",
                    new 
                    { 
                        GardenId = _fixture.GetGardenId().Value,
                        GardenBedId = _fixture.GetGardenBedId().Value,
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

    [Fact, Order(4.2)]
    public async Task T42000_DescribePlant()
    {
        await _fixture
            .Command(
                "/commands/DescribePlantCommand",
                new
                {
                    GardenId = _fixture.GetGardenId().Value,
                    GardenBedId = _fixture.GetGardenBedId().Value,
                    PlantId = _fixture.GetPlantId().Value,
                    Name = "Dutch Carrots",
                    Description = "Seedling purchased from bunnings",
                    ImageUri = "https://cdn/images/plant.jpg",
                    ImageDescription = "Just planted"
                });
    }

    [Fact, Order(4.3)]
    public async Task T43000_GetAllPlants_AfterDescribe()
    {
        var plants =
            await _fixture
                .Query<IEnumerable<PlantDescription>>(
                    "/queries/GetAllPlantDescriptionsQuery",
                    new
                    {
                        GardenId = _fixture.GetGardenId().Value,
                        GardenBedId = _fixture.GetGardenBedId().Value,
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

    [Fact, Order(5.0)]
    public async Task T50000_MoveGarden()
    {
        await _fixture
            .Command(
                "/commands/MoveGardenCommand",
                new
                {
                    GardenId = _fixture.GetGardenId().Value,
                    Center = new Point(1.5, 0.5)
                });
    }

    [Fact, Order(5.1)]
    public async Task T51000_GetGardens_AfterMove()
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

    [Fact, Order(5.2)]
    public async Task T52000_MoveGardenBed()
    {
        await _fixture
            .Command(
                "/commands/ReshapeGardenBedCommand",
                new
                {
                    GardenId = _fixture.GetGardenId().Value,
                    GardenBedId = _fixture.GetGardenBedId().Value,
                    Shape = new
                    {
                        Type = ShapeType.Rectangular,
                        Point = new Point(2.5, 0.5),
                        Rotation = 0,
                        Width = 1.0,
                        Height = 3.0
                    }
                });
       ;
    }

    [Fact, Order(5.3)]
    public async Task T53000_GetAllGardenBeds_AfterMoveGardenBed()
    {
        var gardenBeds =
            await _fixture
                .Query<IEnumerable<GardenBedDescriptor>>(
                    "/queries/GetAllGardenBedDescriptionsQuery",
                    new { GardenId = _fixture.GetGardenId().Value });

        gardenBeds
            .Should()
            .SatisfyRespectively(
                first =>
                {
                    first.Center.Should().BeEquivalentTo(new { X = 2.5, Y = 0.5 });
                });
    }

    [Fact, Order(5.4)]
    public async Task T54000_MovePlant()
    {
        await _fixture
            .Command(
                "/commands/ReshapePlantCommand",
                new
                {
                    GardenId = _fixture.GetGardenId().Value,
                    GardenBedId = _fixture.GetGardenBedId().Value,
                    PlantId = _fixture.GetPlantId().Value,
                    Shape = new 
                    {
                        Type = ShapeType.Circular,
                        Point = new { X = 3.0, Y = 0 },
                        Rotation = 0,
                        Radius = 0.5,
                    },
                });
    }

    [Fact, Order(5.5)]
    public async Task T55000_GetAllPlants_AfterMovePlant()
    {
        var plants =
            await _fixture
                .Query<IEnumerable<PlantDescription>>(
                    "/queries/GetAllPlantDescriptionsQuery",
                    new
                    {
                        GardenId = _fixture.GetGardenId().Value,
                        GardenBedId = _fixture.GetGardenBedId().Value
                    });

        plants
            .Should()
            .SatisfyRespectively(
                first =>
                {
                    first.Center.Should().BeEquivalentTo(new { X = 3.0, Y = 0.0 });
                });
    }

    [Fact, Order(6.0)]
    public async Task T60000_GetGardenMap()
    {
        var map =
            await _fixture
                .Query<GardenMap>(
                    "/queries/GetGardenMapQuery",
                    new
                    {
                        GardenId = _fixture.GetGardenId().Value
                    });

        map.Should().NotBeNull();
    }

    [Fact, Order(7.0)]
    public async Task T70000_RemovePlant()
    {
        await _fixture
            .Command(
                "/commands/RemovePlantCommand",
                new
                {
                    GardenId = _fixture.GetGardenId().Value,
                    GardenBedId = _fixture.GetGardenBedId().Value,
                    PlantId = _fixture.GetPlantId().Value
                });
    }

    [Fact, Order(7.1)]
    public async Task T71000_GetAllPlants_AfterRemovePlant()
    {
        var plants =
            await _fixture
                .Query<IEnumerable<PlantDescription>>(
                    "/queries/GetAllPlantDescriptionsQuery",
                    new
                    {
                        GardenId = _fixture.GetGardenId().Value,
                        GardenBedId = _fixture.GetGardenBedId().Value
                    });

        plants
            .Should()
            .BeEmpty();
    }

    [Fact, Order(7.2)]
    public async Task T72000_RemoveGardenBed()
    {
        await _fixture
            .Command(
                "/commands/RemoveGardenBedCommand",
                new
                {
                    GardenId = _fixture.GetGardenId().Value,
                    GardenBedId = _fixture.GetGardenBedId().Value
                });
    }

    [Fact, Order(7.3)]
    public async Task T73000_GetAllGardenBeds_AfterRemoveGardenBed()
    {
        var gardenBeds =
            await _fixture
                .Query<IEnumerable<GardenBedDescriptor>>(
                    "/queries/GetAllGardenBedDescriptionsQuery",
                    new
                    {
                        GardenId = _fixture.GetGardenId().Value
                    });

        gardenBeds
            .Should()
            .BeEmpty();
    }

    [Fact, Order(7.4)]
    public async Task T74000_RemoveGarden()
    {
        await _fixture
            .Command(
                "/commands/RemoveGardenCommand",
                new
                {
                    GardenId = _fixture.GetGardenId().Value,
                });
    }

    [Fact, Order(7.5)]
    public async Task T75000_GetAllGardens_AfterRemoveGarden()
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
