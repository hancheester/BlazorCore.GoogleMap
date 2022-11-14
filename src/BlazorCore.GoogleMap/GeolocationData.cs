namespace BlazorCore.GoogleMap;

public sealed class GeolocationData : GeolocationCoordinate
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
