using System.Globalization;

namespace BlazorCore.GoogleMap;

public class GeolocationCoordinate
{
	public double? Latitude { get; init; }
	public double? Longitude { get; init; }
	public bool HasCoordinates => Latitude.HasValue && Longitude.HasValue;

	public GeolocationCoordinate(double? latitude, double? longitude)
	{
		Latitude = latitude;
		Longitude = longitude;
	}

	public override string ToString()
	{
		return HasCoordinates
			? $"{Latitude.Value.ToString(CultureInfo.InvariantCulture)},{Longitude.Value.ToString(CultureInfo.InvariantCulture)}"
			: string.Empty;
	}
}