using BlazorCore.GoogleMap;
using BlazorCore.JSInterop.Geolocation;
using Microsoft.AspNetCore.Components;
using System.Diagnostics;
using System.Dynamic;

namespace BlazorCore.GoogleMap;

public partial class GoogleMap : IAsyncDisposable
{
    [Inject] public IGoogleMapService GoogleMapService { get; set; }
    [Inject] public IGeolocationService GeolocationService { get; set; }

    [Parameter] public string ApiKey { get; set; }
    [Parameter] public string? BackgroundColor
    {
        get => _backgroundColor;
        set
        {
            if (_mapInitialized && value != _backgroundColor)
            {
                dynamic options = new ExpandoObject();
                options.backgroundColor = _backgroundColor;
                InvokeAsync(async () => await GoogleMapService.SetOptionsAsync(options));
            }

            _backgroundColor = value;
        }
    }
    [Parameter] public int? ControlSize
    {
        get => _controlSize;
        set
        {
            if (_mapInitialized && value != _controlSize)
            {
                dynamic options = new ExpandoObject();
                options.controlSize = _controlSize;
                InvokeAsync(async () => await GoogleMapService.SetOptionsAsync(options));
            }

            _controlSize = value;
        }
    }
    [Parameter] public string? Width
    {
        get => _width;
        set
        {
            if (_mapInitialized && value != _width)
            {
                InvokeAsync(async () => await GoogleMapService.ResizeMapAsync());
            }

            _width = value;
        }
    }
    [Parameter] public string? Height
    {
        get => _height;
        set
        {
            if (_mapInitialized && value != _height)
            {
                InvokeAsync(async () => await GoogleMapService.ResizeMapAsync());
            }

            _height = value;
        }
    }
    [Parameter] public byte? Zoom
    {
        get => _zoomLevel;
        set
        {
            if (_mapInitialized && value != _zoomLevel)
            {
                InvokeAsync(async () => await GoogleMapService.SetZoomAsync(_zoomLevel!.Value));
            }

            _zoomLevel = value;
        }
    }
    [Parameter] public GoogleLibrary GoogleLibrary { get; set; } = GoogleLibrary.None;
    [Parameter] public bool? KeyboardShortcuts { get; set; }
    [Parameter] public bool? StreetViewControl { get; set; }
    [Parameter] public bool? FullscreenControl { get; set; }
    [Parameter] public GestureHandling? GestureHandling { get; set; }
    [Parameter] public bool? ZoomControl { get; set; }
    [Parameter] public ZoomControlOptions? ZoomControlOptions { get; set; }
    [Parameter] public bool? PanControl { get; set; }
    [Parameter] public bool? MapTypeControl { get; set; }
    [Parameter] public MapTypeControlOptions? MapTypeControlOptions { get; set; }
    [Parameter] public OverlayType? DrawingMode
    {
        get => _drawingMode;
        set
        {
            if (_mapInitialized && value != _drawingMode)
            {
                InvokeAsync(async () => await GoogleMapService.SetDrawingModeAsync(value));
            }

            _drawingMode = value;
        }
    }
    [Parameter] public bool? DrawingControl { get; set; }
    [Parameter] public PolygonOptions? PolygonOptions { get; set; } = null;
    [Parameter] public EventCallback<string> OnMapInitialized { get; set; }
    [Parameter] public EventCallback<GeolocationData> OnCurrentLocationDetected { get; set; }
    [Parameter] public EventCallback<Shape> OnDrawingPolygonCompleted { get; set; }
    [Parameter] public EventCallback<Shape> OnPolygonUpdated { get; set; }
    [Parameter] public EventCallback OnAfterRendered { get; set; }
    [Parameter] public bool CenterCurrentLocationOnLoad { get; set; } = false;
    [Parameter] public bool AnimateCenterChange { get; set; } = true;
    [Parameter] public GeolocationData? Center
    {
        get => _center;
        set
        {
            if (_mapInitialized && !_isDragging && _center?.ToString() != value?.ToString())
            {
                InvokeAsync(async () =>
                {
                    if (Center?.HasCoordinates ?? false)
                    {
                        if (AnimateCenterChange)
                            await GoogleMapService.PanToAsync(Center.Latitude.Value, Center.Longitude.Value);
                        else
                            await GoogleMapService.SetCenterAsync(Center.Latitude.Value, Center.Longitude.Value);
                    }
                    else if (!string.IsNullOrWhiteSpace(Center?.Address))
                    {
                        if (AnimateCenterChange)
                            await GoogleMapService.PanToAsync(Center.Address);
                        else
                            await GoogleMapService.SetCenterAsync(Center.Address);
                    }
                });
            }

            _center = value;
        }
    }
    [Parameter] public Bounds? Bounds
    {
        get => _bounds;
        set
        {
            if (_mapInitialized && value != _bounds)
            {
                InvokeAsync(async () => await GoogleMapService.FitBoundsAsync(value!.East, value.North, value.South, value.West));
            }
            _bounds = value;
        }
    }

    private string _mapContainerId = $"map_{Guid.NewGuid():n}";
    private bool _mapInitialized = false;
    private bool _isDragging = false;
    private GeolocationData? _center;
    private string? _width = "400px";
    private string? _height = "300px";
    private byte? _zoomLevel = 8;
    private string? _backgroundColor;
    private int? _controlSize;
    private OverlayType? _drawingMode;
    private Bounds? _bounds;

    protected override async Task OnInitializedAsync()
    {
        await GoogleMapService.InitMapAsync(
            apiKey: ApiKey,
            googleLibrary: GoogleLibrary,
            mapContainerId: _mapContainerId,
            mapInitializedCallback: async (mapContainerId) =>
            {
                if (CenterCurrentLocationOnLoad)
                    await CenterCurrentLocationOnMapAsync();

                await SetOptionsAsync();

                _mapInitialized = true;
                StateHasChanged();

                if (OnMapInitialized.HasDelegate)
                {
                    await OnMapInitialized.InvokeAsync(_mapContainerId);
                }
            },
            polygonUpdated: async (id, bounds) =>
            {
                if (OnPolygonUpdated.HasDelegate)
                {
                    await OnPolygonUpdated.InvokeAsync(new Shape
                    {
                        Id = id,
                        Bounds = bounds
                    });
                }
            },
            drawingPolygonCompletedCallback: async (id, bounds) =>
            {
                if (OnDrawingPolygonCompleted.HasDelegate)
                {
                    await OnDrawingPolygonCompleted.InvokeAsync(new Shape
                    {
                        Id = id,
                        Bounds = bounds
                    });
                }
            });
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (OnAfterRendered.HasDelegate)
            {
                await OnAfterRendered.InvokeAsync();
            }
        }
    }

    public async Task ClearAllPolygonsAsync()
    {
        await GoogleMapService.ClearAllPolygonsAsync();
    }

    public async Task<ulong> ComputeAreaAsync(string shapeId)
    {
        return await GoogleMapService.ComputeAreaAsync(shapeId);
    }

    public async Task FitBoundsAsync(double eastLongitude, double northLatitude, double southLatitude, double westLongitude)
    {
        await GoogleMapService.FitBoundsAsync(eastLongitude, northLatitude, southLatitude, westLongitude);
    }

    public async Task DrawPolygonAsync(Shape shape, PolygonOptions? polygonOptions = null, bool editable = false)
    {
        if (polygonOptions is not null)
        {
            await GoogleMapService.DrawPolygonAsync(shape, polygonOptions, editable);
        }
        else if (PolygonOptions is not null)
        {
            await GoogleMapService.DrawPolygonAsync(shape, PolygonOptions, editable);
        }
        else
        {
            await GoogleMapService.DrawPolygonAsync(shape, editable: editable);
        }
    }

    public async Task MaskMapAsync(Shape? shape = null, PolygonOptions? polygonOptions = null)
    {
        await GoogleMapService.MaskMapAsync(shape, polygonOptions);
    }

    public async Task CenterCurrentLocationOnMapAsync()
    {
        await GeolocationService.GetCurrentPositionAsync(LocationResult, false, TimeSpan.FromSeconds(10));
    }

    private async Task SetOptionsAsync()
    {
        await SetMapOptionsAsync();
        await SetDrawingOptionsAsync();
    }

    private async Task SetDrawingOptionsAsync()
    {
        if (GoogleLibrary.HasFlag(GoogleLibrary.Drawing))
        {
            dynamic drawingOptions = new ExpandoObject();

            if (DrawingControl.HasValue)
                drawingOptions.drawingControl = DrawingControl.Value;

            if (PolygonOptions is not null)
                drawingOptions.polygonOptions = PolygonOptions;

            if (((IDictionary<string, object>)drawingOptions).Keys.Count > 0)
                await InvokeAsync(async () => await GoogleMapService.SetDrawingOptionsAsync(drawingOptions));

            await InvokeAsync(async () => await GoogleMapService.SetDrawingModeAsync(DrawingMode));
        }
    }

    private async Task SetMapOptionsAsync()
    {
        if (!string.IsNullOrEmpty(_width) || !string.IsNullOrEmpty(_height))
            await InvokeAsync(async () => await GoogleMapService.ResizeMapAsync());

        if (_zoomLevel.HasValue)
            await InvokeAsync(async () => await GoogleMapService.SetZoomAsync(_zoomLevel.Value));

        dynamic options = new ExpandoObject();

        if (KeyboardShortcuts.HasValue)
            options.keyboardShortcuts = KeyboardShortcuts.Value;

        if (StreetViewControl.HasValue)
            options.streetViewControl = StreetViewControl.Value;

        if (FullscreenControl.HasValue)
            options.fullscreenControl = FullscreenControl.Value;

        if (PanControl.HasValue)
            options.panControl = PanControl.Value;

        if (GestureHandling.HasValue)
            options.gestureHandling = GestureHandling.Value;

        if (MapTypeControl.HasValue)
            options.mapTypeControl = MapTypeControl.Value;

        if (MapTypeControlOptions is not null)
            options.mapTypeControlOptions = MapTypeControlOptions;

        if (ZoomControl.HasValue)
            options.zoomControl = ZoomControl.Value;

        if (ZoomControlOptions is not null)
            options.zoomControlOptions = ZoomControlOptions;

        if (_controlSize.HasValue) options.controlSize = _controlSize;

        if (((IDictionary<string, object>)options).Keys.Count > 0)
            await InvokeAsync(async () => await GoogleMapService.SetOptionsAsync(options));

        if (Bounds is not null)
            await InvokeAsync(async () => await GoogleMapService.FitBoundsAsync(Bounds.East, Bounds.North, Bounds.South, Bounds.West));
    }

    private async Task LocationResult(GeolocationResult pos)
    {
        if (pos?.IsSuccess ?? false)
        {
            await GoogleMapService.SetCenterAsync(pos.Coordinates.Latitude, pos.Coordinates.Longitude);
            Center = new GeolocationData(pos.Coordinates.Latitude, pos.Coordinates.Longitude);

            if (OnCurrentLocationDetected.HasDelegate)
            {
                await OnCurrentLocationDetected.InvokeAsync(Center);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (GoogleMapService is not null)
        {
            await GoogleMapService.DisposeAsync();
        }

        if (GeolocationService is not null)
        {
            await GeolocationService.DisposeAsync();
        }
    }
}