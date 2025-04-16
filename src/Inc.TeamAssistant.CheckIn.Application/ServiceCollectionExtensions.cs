using Inc.TeamAssistant.CheckIn.Application.CommandHandlers.AddLocationToMap.Services;
using Inc.TeamAssistant.CheckIn.Application.QueryHandlers.GetLocations.Converters;
using Inc.TeamAssistant.CheckIn.Application.QueryHandlers.GetLocations.Services;
using Inc.TeamAssistant.Primitives.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.CheckIn.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCheckInApplication(
        this IServiceCollection services,
        string connectToMapLinkTemplate)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectToMapLinkTemplate);

        services
            .AddSingleton(sp => ActivatorUtilities.CreateInstance<MapLinksBuilder>(sp, connectToMapLinkTemplate))
            .AddSingleton<StatsByPersonBuilder>()
            .AddSingleton<LocationConverter>()
            
            .AddSingleton<ICommandCreator, AddLocationToMapCommandCreator>();

        return services;
    }
}