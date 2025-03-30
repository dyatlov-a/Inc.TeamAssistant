using Inc.TeamAssistant.CheckIn.Application.Contracts;
using Inc.TeamAssistant.Primitives.Features.PersonStats;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.CheckIn.DataAccess;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCheckInDataAccess(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddSingleton<ILocationsRepository, LocationsRepository>()
            .AddSingleton<IPersonStatsProvider, CheckInStatsProvider>();

        return services;
    }
}