using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyGardenPatch.Tests.Common;

public enum Descriminator
{
    TypeA,
    TypeB
}

[System.Text.Json.Serialization.JsonConverter(typeof(BaseTypeJsonConverter))]
public abstract record BaseType(Descriminator Type);

public record TypeA : BaseType
{
    public TypeA(Descriminator type, string name) : base(type)
    {
        Name = name;
    }

    public string Name { get; }
}

public record TypeB : BaseType
{
    public TypeB(Descriminator type, int age, string somethingElse) : base(type)
    {
        Age = age;
        SomethingElse = somethingElse;
    }

    public int Age { get; }
    public string SomethingElse { get; }
}

public record ComplexType(string State, BaseType Data);

public class BaseTypeJsonConverter : DescriminatedJsonConverter<BaseType>
{
    public BaseTypeJsonConverter() : base(
        (id) => id switch
        {
            "0" => typeof(TypeA),
            "1" => typeof(TypeB),
            _ => null
        } )
    {
    }
}

public class DescriminatedJsonConversionTests
{
    [Fact]
    public void CanConvertToJsonAndBack()
    {
        BaseType original = new TypeA(Descriminator.TypeA, "Hello");

        var json = JsonSerializer.Serialize(original);

        var convertedFromJson = JsonSerializer.Deserialize<BaseType>(json);

        convertedFromJson
            .Should()
            .BeEquivalentTo(original);
    }

    [Fact]
    public void WorksWithCamelCase()
    {
        BaseType original = new TypeA(Descriminator.TypeA, "Hello");

        var json = JsonSerializer.Serialize(original, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var convertedFromJson = JsonSerializer.Deserialize<BaseType>(json);

        convertedFromJson
            .Should()
            .BeEquivalentTo(original);
    }

    [Fact]
    public void WorksWithinComplexTypes()
    {
        var complex = new ComplexType("Test 1", new TypeA(Descriminator.TypeA, "Type A"));

        var json = JsonSerializer.Serialize(complex);

        var convertedFromJson = JsonSerializer.Deserialize<ComplexType>(json)!;

        convertedFromJson.Data.Should().BeOfType<TypeA>();

        convertedFromJson
            .Should()
            .BeEquivalentTo(complex);
    }

    [Fact]
    public void WorksWithinComplexTypes2()
    {
        var complex = new ComplexType("Test 1", new TypeB(Descriminator.TypeB, 40, "Type B"));

        var json = JsonSerializer.Serialize(complex);

        var convertedFromJson = JsonSerializer.Deserialize<ComplexType>(json)!;

        convertedFromJson.Data.Should().BeOfType<TypeB>();

        convertedFromJson
            .Should()
            .BeEquivalentTo(complex);
    }

    [Fact]
    public void CanDeseralizeShape()
    {
        var json = "{\"type\":\"Circular\",\"point\":{\"x\":0.5,\"y\":0.5},\"rotation\":0,\"radius\":2}";

        var circle = (Circular)JsonSerializer.Deserialize(json, typeof(Circular), 
            new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true, 
                Converters = 
                {
                    new JsonStringEnumConverter(allowIntegerValues: true)
                }
            })!;

        circle.Type.Should().Be(ShapeType.Circular);
    }
}
