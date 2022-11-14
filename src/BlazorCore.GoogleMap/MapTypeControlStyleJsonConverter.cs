using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorCore.GoogleMap;

public class MapTypeControlStyleJsonConverter : JsonConverter<MapTypeControlStyle>
{
    public override MapTypeControlStyle Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) => Enum.Parse<MapTypeControlStyle>(reader.GetString());

    public override void Write(
        Utf8JsonWriter writer,
        MapTypeControlStyle style,
        JsonSerializerOptions options) =>
            writer.WriteNumberValue((int)style);
}