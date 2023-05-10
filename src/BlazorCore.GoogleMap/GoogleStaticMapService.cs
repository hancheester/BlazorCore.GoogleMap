namespace BlazorCore.GoogleMap;

public sealed class GoogleStaticMapService : IGoogleStaticMapService
{
    public string GetMapUrl(
        GoogleStaticMapPath? path = null, 
        IEnumerable<GoogleStaticMapMarker>? markers = null,
        GeolocationData? center = null,
        IEnumerable<GeolocationData>? visibleLocations = null,
        byte zoomLevel = 12,
        int width = 400,
        int height = 300,
        bool highResolution = false,
        MapType mapType = MapType.roadmap,
        GoogleStaticMapImageFormats imageFormat= GoogleStaticMapImageFormats.Png,
        string language = "",
        string region = "",
        string style = "",
        string apiKey = "",
        string signature = "")
    {
        var baseUrl = "https://maps.googleapis.com/maps/api/staticmap?";
        var query = new List<string>();
        if ((path == null) && (!markers?.Any() ?? true))
        {
            query.Add($"center={center?.ToString()}");
            if (!visibleLocations?.Any() ?? true)
            {
                query.Add($"zoom={zoomLevel}");
            }
        }

        query.Add($"size={width}x{height}");
        query.Add($"scale={(highResolution ? 2 : 1)}");
        query.Add($"maptype={mapType.ToString().ToLower()}");
        query.Add($"format={imageFormat.ToString().ToLower()}");
        if (!string.IsNullOrWhiteSpace(language))
        {
            query.Add($"language={language}");
        }

        if (!string.IsNullOrWhiteSpace(region))
        {
            query.Add($"region={region}");
        }

        //Features
        if (path != null)
        {
            query.Add(path.ToString());
        }

        if (visibleLocations?.Any() ?? false)
        {
            var loc = visibleLocations.Select(s => s.ToString()).Where(x => !string.IsNullOrWhiteSpace(x));
            query.Add($"visible={string.Join("|", loc)}");
        }

        if (markers?.Any() ?? false)
        {
            var _markers = markers.Select(s => s.ToString()).Where(x => !string.IsNullOrWhiteSpace(x));
            query.Add(string.Join('&', _markers));
        }

        if (!string.IsNullOrWhiteSpace(style))
        {
            query.Add($"style={style}");
        }

        //Auth
        query.Add($"key={apiKey}");
        if (!string.IsNullOrWhiteSpace(signature))
        {
            query.Add($"signature={signature}");
        }

        var url = baseUrl + string.Join('&', query.Where(x => !string.IsNullOrWhiteSpace(x)));
        return url;
    }
}