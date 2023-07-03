using System.Dynamic;

namespace BlazorCore.GoogleMap;

public interface IGoogleMapService : IAsyncDisposable
{
    Task ClearAllCirclesAsync();
    Task ClearAllPolygonsAsync();
    Task ClearCustomOverlayAsync();
    Task<ulong> ComputeAreaAsync(string shapeId);
    Task DrawAdvancedMarkerAsync(string markerId, LatLng position, string content);
    Task DrawCircleAsync(Shape shape, CircleOptions? circleOptions = null);
    Task DrawPolygonAsync(Shape shape, PolygonOptions? polygonOptions = null, bool editable = false);
    Task FitBoundsAsync(double eastLongitude, double northLatitude, double southLatitude, double westLongitude);
    Task InitMapAsync(
        string apiKey, 
        string mapContainerId, 
        GoogleLibrary googleLibrary = GoogleLibrary.None,
        MapType mapType = MapType.roadmap,
        Func<string, Task>? mapInitializedCallback = null,
        Func<string, LatLng[], Task>? polygonUpdated = null,
        Func<string, LatLng[], Task>? drawingPolygonCompletedCallback = null);
    Task MaskMapAsync(Shape? shape = null, PolygonOptions? polygonOptions = null);
    Task PanToAsync(double latitude, double longitude);
    Task PanToAsync(string address);
    Task ResizeMapAsync();
    Task SetAdvancedMarkerContentAsync(string markerid, string content);
    Task SetAdvancedMarkerPositionAsync(string markerid, LatLng position);
    Task SetCenterAsync(double latitude, double longitude);
    Task SetCenterAsync(string address);
    Task SetCircleCenterAsync(string circleId, LatLng center);
    Task SetCustomOverlayAsync(string imageSrc, double southWestLatitude, double southWestLongitude, double northEastLatitude, double northEastLongitude);
    Task SetDrawingModeAsync(OverlayType? drawingMode = null);
    Task SetDrawingOptionsAsync(ExpandoObject options);
    Task SetOptionsAsync(ExpandoObject options);
    Task SetZoomAsync(byte zoomLevel);
}
