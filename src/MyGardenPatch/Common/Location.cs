using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace MyGardenPatch.Common;

public enum LocationType
{
    Point,
    Boundary
}

public record Point
{
    public double X { get; }
    public double Y { get; }

    [JsonConstructor]
    public Point(double x, double y)
    {
        X = x;
        Y = y;
    }

    public static implicit operator Point(string str)
    {
        var regex = new Regex(@"(-?\d+(\.\d+)?),(-?\d+(\.\d+)?)");

        if (!regex.IsMatch(str)) throw new InvalidOperationException("Can't convert string to point");

        var match = regex.Match(str);

        var x = double.Parse(match.Groups[1].Value);
        var y = double.Parse(match.Groups[3].Value);

        return new Point(x, y);
    }

    internal Point Transform(Transformation transformation)
    {
        var x = X * transformation.M11 + Y * transformation.M21 + transformation.M31;
        var y = X * transformation.M12 + Y * transformation.M22 + transformation.M32;

        return new Point(x, y);
    }
}

public record Location
{
    [JsonConstructor]
    public Location(LocationType Type, IEnumerable<Point> Points)
    {
        this.Type = Type;
        this.Points = Points;
    }

    public Location(double lat, double lng) : this(LocationType.Point, new[] { new Point(lat, lng) }) { }

    public static Location Default => new Location(LocationType.Point, new[] { new Point(0, 0) });

    public LocationType Type { get; }

    public IEnumerable<Point> Points { get; }
    public Point Center => this switch
    {
        { Type: LocationType.Point } => Points.First(),
        { Type: LocationType.Boundary } => new Point(
            Points.Select(p => p.X).Average(),
            Points.Select(p => p.Y).Average()),

        _ => throw new NotImplementedException()
    };

    internal Location Transform(Transformation transformation)
    {
        return new Location(Type, Points.Select(p => p.Transform(transformation)).ToArray());
    }

    public static implicit operator Location(string str)
    {
        var coordinateRegex = new Regex(@"(-?\d+(\.\d+)?),(-?\d+(\.\d+)?)");

        if (coordinateRegex.IsMatch(str))
        {
            var match = coordinateRegex.Match(str);

            return new Location(
                double.Parse(match.Groups[1].Value),
                double.Parse(match.Groups[3].Value));
        }

        throw new ArgumentException($"Cannot convert given string '{str}' to location");
    }
}
