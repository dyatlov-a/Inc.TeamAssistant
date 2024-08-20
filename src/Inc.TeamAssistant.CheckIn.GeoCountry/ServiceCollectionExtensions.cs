using Inc.TeamAssistant.CheckIn.Application.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.CheckIn.GeoCountry;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCheckInGeoCountry(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddSingleton<FileLoader>()
            .AddSingleton<GeoJsonParser>()
            .AddSingleton<IReverseLookup, ReverseLookup>();

        return services;
    }
}