using System.Text.Json.Serialization;

namespace BlazorCore.GoogleMap;

public class ZoomControlOptions
{
    [JsonConverter(typeof(ControlPositionJsonConverter))]
    public ControlPosition? Position { get; set; }
}

public class MapTypeControlOptions
{
    [JsonConverter(typeof(ControlPositionJsonConverter))]
    public ControlPosition? Position { get; set; }

    [JsonConverter(typeof(MapTypeControlStyleJsonConverter))]
    public MapTypeControlStyle? Style { get; set; }
}
