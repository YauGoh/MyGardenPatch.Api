namespace MyGardenPatch.Tests.TestData;

internal static class GardenTestData
{
    public const string UnknownGardenId = "{263005A8-E55D-49EB-BCE1-109CACD19E63}";

    public const string PeterGardenId = "{1928CCB7-D1C9-4EBD-B917-21CFC313730D}";

    public static Garden PeterGarden => new Garden (
        new Guid(PeterGardenId),
        new Guid(UserTestData.PeterParkerUserId),
        "Peter's Garden",
        "Growing veges",
        new Uri("https://cdn/image.jpg"),
        "My garden",
        new DateTime(2022, 1, 1))
    {
        Center = new Point(1, 1)
    };
}
