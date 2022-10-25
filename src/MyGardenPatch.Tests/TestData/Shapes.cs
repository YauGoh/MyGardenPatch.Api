namespace MyGardenPatch.Tests.TestData;

internal static class Shapes
{
    internal static Rectangular Rectangle_1x25 => new Rectangular
    {
        Type = ShapeType.Rectangular,
        Point = new Point(0, 0),
        Rotation = 0,
        Width = 1,
        Height = 2.5
    };

    internal const string Rectangle_1x25_str = "R 0,0 0 1,2.5";

    internal static Circular Circular_2 => new Circular
    {
        Type = ShapeType.Circular,
        Point = new Point(0, 0),
        Rotation = 0,
        Radius = 2
    };
}
