namespace BlazorCore.GoogleMap;

public sealed class Shape
{
	public string Id { get; set; }
	public LatLng[] Bounds { get; set; }
}