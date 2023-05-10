using Microsoft.JSInterop;
using System.Dynamic;

namespace BlazorCore.GoogleMap;

public sealed class GoogleMapService : IGoogleMapService
{
    private readonly IJSRuntime _jsRuntime;
    private IJSObjectReference _mapsJs;
    private DotNetObjectReference<GoogleMapEventInfo> _eventDotNetRef;

    public string MapContainerId { get; private set; }

    public GoogleMapService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task InitMapAsync(
        string apiKey, 
        string mapContainerId,
        GoogleLibrary googleLibrary = GoogleLibrary.None,
        MapType mapType = MapType.roadmap,
        Func<string, Task>? mapInitializedCallback = null,
        Func<string, LatLng[], Task>? polygonUpdated = null,
        Func<string, LatLng[], Task>? drawingPolygonCompletedCallback = null)
    {
        if (MapContainerId == mapContainerId)
            return;

        MapContainerId = mapContainerId;

        await CheckJsObjectAsync();

        var eventInfo = new GoogleMapEventInfo(
            mapContainerId,
            mapInitializedCallback: mapInitializedCallback,
            polygonUpdatedCallback: polygonUpdated,
            drawingPolygonCompletedCallback: drawingPolygonCompletedCallback);

        _eventDotNetRef = DotNetObjectReference.Create(eventInfo);

        var libraries = new List<string>();
        if (googleLibrary.HasFlag(GoogleLibrary.Drawing))
        {
            libraries.Add(nameof(GoogleLibrary.Drawing).ToLower());
        }
        if (googleLibrary.HasFlag(GoogleLibrary.Geometry))
        {
            libraries.Add(nameof(GoogleLibrary.Geometry).ToLower());
        }

        await _mapsJs.InvokeVoidAsync("init", apiKey, string.Join(",", libraries), mapType, mapContainerId, _eventDotNetRef);
    }

    public async Task SetCenterAsync(double latitude, double longitude)
    {
        await CheckJsObjectAsync();
        await _mapsJs.InvokeVoidAsync("setCenterCoords", MapContainerId, latitude, longitude);
    }

    public async Task PanToAsync(double latitude, double longitude)
    {
        await CheckJsObjectAsync();
        await _mapsJs.InvokeVoidAsync("panToCoords", MapContainerId, latitude, longitude);
    }

    public async Task PanToAsync(string address)
    {
        await CheckJsObjectAsync();
        await _mapsJs.InvokeVoidAsync("panToAddress", MapContainerId, address);
    }

    public async Task SetCenterAsync(string address)
    {
        await CheckJsObjectAsync();
        await _mapsJs.InvokeVoidAsync("setCenterAddress", MapContainerId, address);
    }

    public async Task ResizeMapAsync()
    {
        await CheckJsObjectAsync();
        await _mapsJs.InvokeVoidAsync("resizeMap", MapContainerId);
    }

    public async Task SetZoomAsync(byte zoom)
    {
        await CheckJsObjectAsync();
        await _mapsJs.InvokeVoidAsync("setZoom", MapContainerId, zoom);
    }

    public async Task SetOptionsAsync(ExpandoObject options)
    {
        await CheckJsObjectAsync();
        await _mapsJs.InvokeVoidAsync("setOptions", MapContainerId, options);
    }

    public async Task SetDrawingOptionsAsync(ExpandoObject options)
    {
        await CheckJsObjectAsync();
        await _mapsJs.InvokeVoidAsync("setDrawingOptions", MapContainerId, options);
    }

    public async Task SetDrawingModeAsync(OverlayType? drawingMode = null)
    {
        await CheckJsObjectAsync();
        await _mapsJs.InvokeVoidAsync("setDrawingMode", MapContainerId, drawingMode);
    }

    public async Task ClearAllPolygonsAsync()
    {
        await CheckJsObjectAsync();
        await _mapsJs.InvokeVoidAsync("clearAllPolygons", MapContainerId);
    }

    public async Task<ulong> ComputeAreaAsync(string shapeId)
    {
        await CheckJsObjectAsync();
        var value = await _mapsJs.InvokeAsync<string>("computeArea", MapContainerId, shapeId);
        return Convert.ToUInt64(value);
    }

    public async Task FitBoundsAsync(double eastLongitude, double northLatitude, double southLatitude, double westLongitude)
    {
        await CheckJsObjectAsync();
        await _mapsJs.InvokeVoidAsync("fitBounds", MapContainerId, eastLongitude, northLatitude, southLatitude, westLongitude);
    }

    public async Task DrawPolygonAsync(Shape shape, PolygonOptions? polygonOptions = null, bool editable = false)
    {
        await CheckJsObjectAsync();
        await _mapsJs.InvokeVoidAsync("drawPolygon", MapContainerId, shape, polygonOptions, editable);
    }

    public async Task MaskMapAsync(Shape? shape = null, PolygonOptions? polygonOptions = null)
    {
        await CheckJsObjectAsync();
        if (polygonOptions == null)
        {
            //Set default
            polygonOptions = new PolygonOptions
            {
                StrokeOpacity = 0,
                StrokeWeight = 0,
                FillColor = "#000000",
                FillOpacity = 0.2,
            };
        }
        await _mapsJs.InvokeVoidAsync("maskMap", MapContainerId, shape, polygonOptions);
    }

    public async Task SetCustomOverlayAsync(string imageSrc, double southWestLatitude, double southWestLongitude, double northEastLatitude, double northEastLongitude)
    {
        await CheckJsObjectAsync();
        await _mapsJs.InvokeVoidAsync("setCustomOverlay", MapContainerId, imageSrc, southWestLatitude, southWestLongitude, northEastLatitude, northEastLongitude);
    }

    public async Task ClearCustomOverlayAsync()
    {
        await CheckJsObjectAsync();
        await _mapsJs.InvokeVoidAsync("clearCustomOverlay", MapContainerId);
    }

    private async Task CheckJsObjectAsync()
    {
        if (_mapsJs is null)
        {
            _mapsJs = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/BlazorCore.GoogleMap/gmap.js");
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_mapsJs is not null)
        {
            await _mapsJs.InvokeVoidAsync("dispose", MapContainerId);
            await _mapsJs.DisposeAsync();
        }

        _eventDotNetRef?.Dispose();
    }
}
