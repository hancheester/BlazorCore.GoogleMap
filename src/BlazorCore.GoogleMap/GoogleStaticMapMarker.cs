namespace BlazorCore.GoogleMap;

public sealed class GoogleStaticMapMarker
{
    public GoogleMapMarkerStyle? Style { get; set; }
    public GoogleMapMarkerCustomIcon? CustomIcon { get; set; }
    public IList<GeolocationData> Locations { get; }
    public bool HasStyleDefined => CustomIcon is not null || Style is not null;

    public GoogleStaticMapMarker()
    {
        Locations = new List<GeolocationData>();
    }

    public override string ToString()
    {
        var loc = Locations.Where(x => !string.IsNullOrWhiteSpace(x?.ToString()))
            .Select(s => s.ToString());

        if (!loc.Any())
        {
            return string.Empty;
        }

        var style = HasStyleDefined
            ? CustomIcon is not null ? CustomIcon?.ToString() : Style?.ToString()
            : null;

        if (!string.IsNullOrWhiteSpace(style))
        {
            style += "|";
        }

        return $"markers={style}{string.Join("|", loc)}";
    }
}