

namespace MyGardenPatch.Common;

/// <summary>
/// Matrix in the form [M11 M12 M13]
///                    [M21 M22 M23]
///                    [M31 M32 M33]
/// </summary>
/// <param name="M11"></param>
/// <param name="M12"></param>
/// <param name="M21"></param>
/// <param name="M22"></param>
[JsonConverter(typeof(TransformationConverter))]
public record Transformation(double M11, double M12, double M13,
                             double M21, double M22, double M23,
                             double M31, double M32, double M33)
{
    public static implicit operator Transformation(double[] matrix) => new Transformation(matrix[0], matrix[1], matrix[2],
                                                                                          matrix[3], matrix[4], matrix[5],
                                                                                          matrix[6], matrix[7], matrix[8]);

    public static implicit operator Transformation(string matrix)
    {
        var doubles = matrix
            .Split(new char[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(str => Double.Parse(str))
            .ToArray();

        return (Transformation)doubles;
    }

    public static implicit operator string(Transformation matrix)
    {
        return $"{matrix.M11} {matrix.M12} {matrix.M13} " +
               $"{matrix.M21} {matrix.M22} {matrix.M23} " +
               $"{matrix.M31} {matrix.M32} {matrix.M33}";

    }
}

public class TransformationConverter : JsonConverter<Transformation>
{
    public override Transformation Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var str = reader.GetString() ?? throw new InvalidOperationException("Transform string expected");

        return (Transformation)str;
    }

    public override void Write(
        Utf8JsonWriter writer,
        Transformation value,
        JsonSerializerOptions options)
    {
        writer.WriteStringValue((string)value);
    }
}
