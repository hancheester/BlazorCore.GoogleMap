using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorCore.GoogleMap;

public static class ServiceProviderExtensions
{
    public static void ConfigureGMapComponent(this IServiceProvider services)
    {
        var jsRuntime = services.GetService<IJSRuntime>();
        var property = typeof(JSRuntime).GetProperty("JsonSerializerOptions", BindingFlags.NonPublic | BindingFlags.Instance);
        var options = Convert.ChangeType(property!.GetValue(jsRuntime, null), typeof(JsonSerializerOptions)) as JsonSerializerOptions;
        options!.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    }
}
