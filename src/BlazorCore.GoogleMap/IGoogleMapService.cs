using System.Dynamic;

namespace BlazorCore.GoogleMap;

public interface IGoogleMapService : IAsyncDisposable
{
    Task ClearAllPolygonsAsync();
    Task<ulong> ComputeAreaAsync(string shapeId);
    Task DrawPolygonAsync(Shape shape, PolygonOptions? polygonOptions = null, bool editable = false);
    Task FitBoundsAsync(double eastLongitude, double northLatitude, double southLatitude, double westLongitude);
    Task InitMapAsync(
        string apiKey, 
        string mapContainerId, 
        GoogleLibrary googleLibrary = GoogleLibrary.None, 
        Func<string, Task>? mapInitializedCallback = null,
        Func<string, LatLng[], Task>? polygonUpdated = null,
        Func<string, LatLng[], Task>? drawingPolygonCompletedCallback = null);
    Task MaskMapAsync(Shape? shape = null, PolygonOptions? polygonOptions = null);
    Task PanToAsync(double latitude, double longitude);
    Task PanToAsync(string address);
    Task ResizeMapAsync();
    Task SetCenterAsync(double latitude, double longitude);
    Task SetCenterAsync(string address);
    Task SetDrawingModeAsync(OverlayType? drawingMode = null);
    Task SetDrawingOptionsAsync(ExpandoObject options);
    Task SetOptionsAsync(ExpandoObject options);
    Task SetZoomAsync(byte zoomLevel);
}
