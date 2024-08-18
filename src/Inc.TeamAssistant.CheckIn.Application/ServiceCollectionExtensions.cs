using Inc.TeamAssistant.CheckIn.Application.CommandHandlers.AddLocationToMap.Services;
using Inc.TeamAssistant.CheckIn.Application.QueryHandlers.GetLocations.Services;
using Inc.TeamAssistant.Primitives.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.CheckIn.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCheckInApplication(this IServiceCollection services, CheckInOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);

        services
            .AddSingleton(options)
            .AddSingleton<LocationConverter>()
            .AddSingleton<ICommandCreator, AddLocationToMapCommandCreator>();

        return services;
    }
}