namespace MyGardenPatch.Common;

public class DescriminatedJsonConverter<T> : JsonConverter<T> where T : class
{
    private readonly string _discriminatorPropertyName;
    private readonly Func<JsonElement, JsonSerializerOptions, string?> _getDescriminatorValue;
    private readonly Func<string, Type?> _getDescriminatedType;

    private static readonly NoopNamingPolicy _noopNamingPolicy = new NoopNamingPolicy();

    private Func<JsonElement, JsonSerializerOptions, string?> defaultGetDecsriminatorFunction =>
        (JsonElement element, JsonSerializerOptions options) =>
        {
            var namingPolicies = new List<JsonNamingPolicy>();

            if (options.PropertyNamingPolicy != null)
            {
                namingPolicies.Add(options.PropertyNamingPolicy);
            }

            namingPolicies.Add(_noopNamingPolicy);
            namingPolicies.Add(JsonNamingPolicy.CamelCase);

            foreach(var namingPolicy in namingPolicies)
            {
                var propertyName = namingPolicy.ConvertName(_discriminatorPropertyName);

                if (element.TryGetProperty(propertyName, out var descriminatedType))
                {
                    return descriminatedType.ValueKind switch
                    {
                        JsonValueKind.Number => descriminatedType.GetInt32().ToString(),
                        JsonValueKind.String => descriminatedType.GetString(),

                        _ => null
                    };
                }
            }

            throw new JsonException($"{_discriminatorPropertyName} is required");
        };
    

    public DescriminatedJsonConverter(
        Func<string, Type?> getDescriminatedType,
        string discriminatorPropertyName = "Type",
        Func<JsonElement, JsonSerializerOptions, string>? getDescriminatorValue = null 
        )
    {
        _getDescriminatedType = getDescriminatedType;

        _discriminatorPropertyName = discriminatorPropertyName;
        _getDescriminatorValue = getDescriminatorValue ?? defaultGetDecsriminatorFunction;
    }

    public override bool CanConvert(Type type)
    {
        return typeof(T).IsAssignableFrom(type);
    }

    public override T? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var value = JsonSerializer.Deserialize<dynamic?>(ref reader, options);

        if (value is null) return null;

        var descriminatorValue = _getDescriminatorValue(value, options);

        var type = _getDescriminatedType.Invoke(descriminatorValue);

        if (type == null) throw new JsonException();

        var json = JsonSerializer.Serialize(value);

        var result = (T)JsonSerializer.Deserialize(
            json, 
            type,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters =
                {
                new JsonStringEnumConverter(allowIntegerValues: true)
                }
            });

        return result;
    }

    public override void Write(
        Utf8JsonWriter writer,
        T? value,
        JsonSerializerOptions options)
    {
        if (value == null) return;

        JsonSerializer.Serialize(writer, (dynamic)value, options);
    }

    internal class NoopNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name) => name;
    }
}
