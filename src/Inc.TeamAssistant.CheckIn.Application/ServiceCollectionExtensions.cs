using Inc.TeamAssistant.CheckIn.Application.CommandHandlers.AddLocationToMap.Services;
using Inc.TeamAssistant.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.CheckIn.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCheckInApplication(this IServiceCollection services, CheckInOptions options)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));
        if (options is null)
            throw new ArgumentNullException(nameof(options));

        services
            .AddSingleton(options)
            .AddSingleton<ICommandCreator, AddLocationToMapCommandCreator>();

        return services;
    }
}