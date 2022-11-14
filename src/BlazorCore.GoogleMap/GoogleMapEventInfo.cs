using Microsoft.JSInterop;

namespace BlazorCore.GoogleMap;

public class GoogleMapEventInfo
{
	private readonly string _mapContainerId;
	private readonly Func<string, Task>? _mapInitializedCallback;
	private readonly Func<byte, Task>? _mapZoomChangedCallback;
	private readonly Func<Rectangle, Task>? _mapResizedCallback;
	private readonly Func<string, LatLng[], Task>? _polygonUpdatedCallback;
	private readonly Func<string, LatLng[], Task>? _drawingPolygonCompletedCallback;

	public GoogleMapEventInfo(
		string mapContainerId,
		Func<string, Task>? mapInitializedCallback = null,
		Func<byte, Task>? mapZoomChangedCallback = null,
        Func<Rectangle, Task>? mapResizedCallback = null,
		Func<string, LatLng[], Task>? polygonUpdatedCallback = null,
		Func<string, LatLng[], Task>? drawingPolygonCompletedCallback = null)
	{
		_mapContainerId = mapContainerId;
		_mapInitializedCallback = mapInitializedCallback;
		_mapZoomChangedCallback = mapZoomChangedCallback;
		_mapResizedCallback = mapResizedCallback;
		_polygonUpdatedCallback = polygonUpdatedCallback;
		_drawingPolygonCompletedCallback = drawingPolygonCompletedCallback;
	}

	[JSInvokable("MapInitialized")]
	public async Task MapInitialized(string mapContainerId)
	{
		if (_mapContainerId != mapContainerId)
		{
			throw new InvalidProgramException($"Invalid map container ID [{mapContainerId}], expecting ID [{_mapContainerId}].");
		}

		if (_mapInitializedCallback != null)
		{
			await _mapInitializedCallback.Invoke(mapContainerId);
		}
	}

	[JSInvokable("MapResized")]
	public async Task MapResized(Rectangle size)
	{
		if (_mapResizedCallback is not null)
		{
			await _mapResizedCallback.Invoke(size);
		}
	}

    [JSInvokable("MapZoomChanged")]
    public async Task MapZoomChanged(byte zoom)
    {
        if (_mapZoomChangedCallback is not null)
        {
            await _mapZoomChangedCallback.Invoke(zoom);
        }
    }

	[JSInvokable("DrawingPolygonCompleted")]
	public async Task DrawingPolygonCompleted(string id, LatLng[] bounds)
	{
		if (_drawingPolygonCompletedCallback is not null)
		{
			await _drawingPolygonCompletedCallback.Invoke(id, bounds);
        }
	}

    [JSInvokable("PolygonUpdated")]
	public async Task PolygonUpdated(string id, LatLng[] bounds)
	{
		if (_polygonUpdatedCallback is not null)
		{
			await _polygonUpdatedCallback.Invoke(id, bounds);
        }
	}
}
