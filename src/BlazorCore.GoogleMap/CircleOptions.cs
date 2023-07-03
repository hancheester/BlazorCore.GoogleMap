namespace BlazorCore.GoogleMap;

public class CircleOptions
{
    public LatLng? Center { get; set; }
    /// <summary>
    /// In meters
    /// </summary>
    public double? Radius { get; set; }
    public bool? Draggable { get; set; }
    public bool? Editable { get; set; }
    public string? FillColor { get; set; }
    public double? FillOpacity { get; set; }
    public string? StrokeColor { get; set; }
    public double? StrokeOpacity { get; set; }
    public int? StrokeWeight { get; set; }
}