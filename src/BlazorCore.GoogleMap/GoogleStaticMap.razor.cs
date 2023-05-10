using BlazorCore.JSInterop.Geolocation;
using Microsoft.AspNetCore.Components;

namespace BlazorCore.GoogleMap;

public partial class GoogleStaticMap : IAsyncDisposable
{
    [Inject] public IGoogleStaticMapService GoogleStaticMapService { get; set; }
    [Inject] public IGeolocationService GeolocationService { get; set; }

    private ElementReference _staticMap;
    public ElementReference InnerElementReference => _staticMap;
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (CenterCurrentLocationOnLoad && firstRender)
        {
            await CenterCurrentLocationOnMapAsync();
        }
    }

    [Parameter] public byte ZoomLevel { get; set; } = 12;
    [Parameter] public int Width { get; set; } = 400;
    [Parameter] public int Height { get; set; } = 300;
    [Parameter] public bool HighResolution { get; set; } = false;
    [Parameter] public MapType MapType { get; set; } = MapType.roadmap;
    [Parameter] public GoogleStaticMapImageFormats ImageFormat { get; set; } = GoogleStaticMapImageFormats.Png;
    [Parameter] public string Language { get; set; } = "";
    [Parameter] public string Region { get; set; } = "";
    [Parameter] public GeolocationData? Center { get; set; }
    [Parameter] public bool CenterCurrentLocationOnLoad { get; set; } = false;
    [Parameter] public string ApiKey { get; set; } = "";
    [Parameter] public string Signature { get; set; } = "";
    [Parameter] public IEnumerable<GoogleStaticMapMarker>? Markers { get; set; }
    [Parameter] public GoogleStaticMapPath? Path { get; set; }
    [Parameter] public IEnumerable<GeolocationData>? VisibleLocations { get; set; }
    [Parameter] public string Style { get; set; } = "";
    [Parameter] public EventCallback<GeolocationData> OnCurrentLocationDetected { get; set; }
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> AllOtherAttributes { get; set; }

    public async Task CenterCurrentLocationOnMapAsync()
    {
        await GeolocationService.GetCurrentPositionAsync(LocationResult, false, TimeSpan.FromSeconds(10));
    }

    private async Task LocationResult(GeolocationResult pos)
    {
        if (pos.IsSuccess)
        {
            Center = new GeolocationData(pos.Coordinates.Latitude, pos.Coordinates.Longitude);
            if (OnCurrentLocationDetected.HasDelegate)
            {
                await OnCurrentLocationDetected.InvokeAsync(Center);
            }
        }
    }

    private string GetMapsUrl()
    {
        return GoogleStaticMapService.GetMapUrl(
            Path, 
            Markers, 
            Center, 
            VisibleLocations, 
            ZoomLevel, 
            Width, 
            Height, 
            HighResolution, 
            MapType, 
            ImageFormat, 
            Language, 
            Region, 
            Style, 
            ApiKey, 
            Signature);
    }

    public async ValueTask DisposeAsync()
    {
        if (GeolocationService is not null)
        {
            await GeolocationService.DisposeAsync();
        }
    }
}