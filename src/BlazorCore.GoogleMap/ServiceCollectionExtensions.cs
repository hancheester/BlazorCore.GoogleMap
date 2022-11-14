using Microsoft.Extensions.DependencyInjection;

namespace BlazorCore.GoogleMap;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGMapComponent(this IServiceCollection services)
    {
        services.AddTransient<IGoogleMapService, GoogleMapService>();
        services.AddOptions();

        return services;
    }
}
