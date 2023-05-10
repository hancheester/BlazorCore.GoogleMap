namespace BlazorCore.GoogleMap;

public interface IGoogleStaticMapService
{
    string GetMapUrl(
        GoogleStaticMapPath? path = null, 
        IEnumerable<GoogleStaticMapMarker>? markers = null, 
        GeolocationData? center = null, 
        IEnumerable<GeolocationData>? visibleLocations = null, 
        byte zoomLevel = 12, 
        int width = 400, 
        int height = 300, 
        bool highResolution = false, 
        MapType mapType = MapType.roadmap, 
        GoogleStaticMapImageFormats imageFormat = GoogleStaticMapImageFormats.Png, 
        string language = "", 
        string region = "", 
        string style = "", 
        string apiKey = "", 
        string signature = "");
}