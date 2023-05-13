namespace MyGardenPatch.Tests.TestData;

internal static class Shapes
{
    internal static Rectangular Rectangle_1x25 => (Rectangular)Rectangle_1x25_str;

    internal const string Rectangle_1x25_str = "R 0,0 0 1,2.5";

    internal static Circular Circular_2 => new Circular(
        ShapeType.Circular,
        new Point(0, 0),
        0,
        2
    );
}
