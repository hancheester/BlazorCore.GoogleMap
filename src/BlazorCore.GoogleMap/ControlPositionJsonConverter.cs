using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorCore.GoogleMap;

public class ControlPositionJsonConverter : JsonConverter<ControlPosition>
{
    public override ControlPosition Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) => Enum.Parse<ControlPosition>(reader.GetString());

    public override void Write(
        Utf8JsonWriter writer,
        ControlPosition position,
        JsonSerializerOptions options) =>
            writer.WriteNumberValue((int)position);
}
