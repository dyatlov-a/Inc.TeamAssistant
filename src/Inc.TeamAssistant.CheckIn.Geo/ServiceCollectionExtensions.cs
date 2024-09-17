using Inc.TeamAssistant.CheckIn.Application.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.CheckIn.Geo;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCheckInGeo(this IServiceCollection services, string webRootPath)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(webRootPath);

        services
            .AddSingleton(sp => ActivatorUtilities.CreateInstance<RegionLoader>(sp, webRootPath))
            .AddSingleton<GeoJsonParser>()
            .AddSingleton<IGeoService, GeoService>();

        return services;
    }
}