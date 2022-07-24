namespace MyGardenPatch.Tests.TestData;

internal static class GardenBedTestData
{
    public const string UnknownGardenBedId = "{FA894BC2-2911-485F-A369-6FC4DFBEA9C5}";

    public const string PeterFrontGardenBedId = "{AB75130D-1ADE-4A00-89E5-45C6BA2F417A}";

    public static GardenBed PeterFrontGardenBed => new GardenBed(
        new Guid(PeterFrontGardenBedId),
        UserTestData.PeterParker.Id,
        GardenTestData.PeterGarden.Id,
        "Front",
        "Nothing Yet, maybe just flowers",
        new Uri("https://images/font.jpg"),
        "My first garden bed",
        new DateTime(2022, 1, 1));

    public const string PeterGardenBedWithCarrotsId = "{038D8698-029B-4E0A-9149-4E64E2919175}";

    public const string CarrotId = "{FEEC7760-7B79-4933-A4FA-F61343852695}";

    public const string UnknownPlantId = "{D95BB64D-3F47-406A-A881-CBC6184781F1}";

    public static GardenBed PeterGardenBedWithCarrots
    {
        get
        {
            var plant = new Plant(new Guid(CarrotId), "Carrots", "Trying dutch carrots", new Uri("https://cdn/image.jpg"), "looking good", new DateTime(2022, 1, 1));
            plant.SetLocation(new Location(LocationType.Point, new[] { new Point(1, 2) }));

            var gardenBed = new GardenBed(
                new Guid(PeterGardenBedWithCarrotsId),
                UserTestData.PeterParker.Id,
                GardenTestData.PeterGarden.Id,
                "Front",
                "Carrots",
                new Uri("https://images/font.jpg"),
                "My first garden bed",
                new DateTime(2022, 1, 1))
            {
                Plants =
                    {
                        plant
                    }
            };

            gardenBed.SetLocation(new Location(1, 1));

            return gardenBed;
        }
    }
}
