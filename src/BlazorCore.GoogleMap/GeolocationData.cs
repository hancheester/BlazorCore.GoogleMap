namespace BlazorCore.GoogleMap;

public class GeolocationData : GeolocationCoordinate
{
	public string? Address { get; init; }

	public GeolocationData(double? latitude, double? longitude)
		: base(latitude, longitude)
	{
	}

	public GeolocationData(string address)
		: base(null, null)
	{
		Address = address;
	}

	public override string ToString()
	{
		return HasCoordinates
			? base.ToString()
			: Address ?? string.Empty;
	}
}

public sealed class GoogleStaticMapPath
{
    public int Weight { get; init; } = 5;
    public string? Color { get; init; }
	public string? FillColor { get; init; }
	public bool Geodesic { get; init; } = false;
    public IList<GeolocationData> Locations { get; }

    public GoogleStaticMapPath(int weight = 5, string? color = null, string? fillColor = null, bool geodesic = false) 
    {
        Locations = new List<GeolocationData>();
        Weight = weight;
		Color = color;
		FillColor = fillColor;
		Geodesic = geodesic;
    }

    public override string ToString()
    {
        var loc = Locations.Where(x => !string.IsNullOrWhiteSpace(x?.ToString())).Select(s => s.ToString());

        if (!loc.Any()) return string.Empty;

        var style = $"path=weight:{Weight}{(string.IsNullOrEmpty(Color) ? "" : $"|color:{Color}")}{(string.IsNullOrEmpty(FillColor) ? "" : $"|fillcolor:{FillColor}")}|geodesic:{Geodesic}|";

		return $"{style}{string.Join("|", loc)}";
    }
}
