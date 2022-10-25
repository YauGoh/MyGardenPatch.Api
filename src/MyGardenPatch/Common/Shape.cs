using System.Text.RegularExpressions;

namespace MyGardenPatch.Common;

public enum ShapeType
{
    Rectangular,
    Circular
};

public class ShapeJsonConverter : DescriminatedJsonConverter<Shape>
{
    public ShapeJsonConverter() 
            : base(MapToShapeType)
    {
    }

    private static Type? MapToShapeType(string value)
    {
        if (!Enum.TryParse<ShapeType>(value, out var shapeType)) return null;

        return shapeType switch
        {
            ShapeType.Rectangular => typeof(Rectangular),
            ShapeType.Circular => typeof(Circular),
            _ => null
        };
    }
}

[JsonConverter(typeof(ShapeJsonConverter))]
public record Shape(ShapeType Type, Point Point, double Rotation)
{
    private static Regex rectangleRegex = new Regex(@"R\s*(-?\d+(\.\d+)?)\s*,\s*(-?\d+(\.\d+)?)\s*(-?\d+(\.\d+)?)\s*(-?\d+(\.\d+)?)\s*,\s*(-?\d+(\.\d+)?)\s*");
    private static Regex circleRegex = new Regex(@"C\s*(-?\d+(\.\d+)?)\s*,\s*(-?\d+(\.\d+)?)\s*(-?\d+(\.\d+)?)\s*(-?\d+(\.\d+)?)\s*");

    public static implicit operator Shape?(string str)
    {
        if (string.IsNullOrEmpty(str)) return default;

        if (rectangleRegex.IsMatch(str))
        {
            var match = rectangleRegex.Match(str);

            var x = double.Parse(match.Groups[1].Value);
            var y = double.Parse(match.Groups[3].Value);
            var rotation = double.Parse(match.Groups[5].Value);
            var width = double.Parse(match.Groups[7].Value);
            var height = double.Parse(match.Groups[9].Value);

            return new Rectangular(
                ShapeType.Rectangular,
                new Point(x, y),
                rotation,
                width,
                height
            );
        }

        if (circleRegex.IsMatch(str))
        {
            var match = rectangleRegex.Match(str);

            var x = double.Parse(match.Groups[1].Value);
            var y = double.Parse(match.Groups[3].Value);
            var rotation = double.Parse(match.Groups[5].Value);
            var radius = double.Parse(match.Groups[7].Value);

            return new Circular(
                ShapeType.Circular,
                new Point(x, y),
                rotation,
                radius 
            );
        }

        throw new ArgumentException("Can't convert shape to string");
    }

    public static implicit operator string(Shape shape) =>
        shape switch
        {
            Rectangular r => $"R {r.Rotation} {r.Point.X},{r.Point.Y} {r.Width},{r.Height} ",
            Circular c => $"C {c.Rotation} {c.Point.X},{c.Point.Y} {c.Radius}",
            _ => throw new ArgumentException("Unknown shape")
        };
}

public record Rectangular : Shape
{
    public Rectangular(ShapeType type, Point point, double rotation, double width, double height) : base(type, point, rotation)
    {
        Width = width;
        Height = height;
    }
    public double Width { get; }
    public double Height { get; }
}

public record Circular : Shape
{
    public Circular(ShapeType type, Point point, double rotation, double radius) : base(type, point, rotation)
    {
        Radius = radius;
    }

    public double Radius { get; }
}
