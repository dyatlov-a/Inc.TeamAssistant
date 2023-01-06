using Inc.TeamAssistant.CheckIn.All.Contracts;
using Inc.TeamAssistant.CheckIn.All.Internal;
using Inc.TeamAssistant.CheckIn.All.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.CheckIn.All;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCheckIn(
        this IServiceCollection services,
        CheckInOptions options,
        string connectionString)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));
        if (options is null)
            throw new ArgumentNullException(nameof(options));
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));

        services
            .AddSingleton<ILocationsRepository>(sp => ActivatorUtilities.CreateInstance<LocationsRepository>(sp, connectionString))
            .AddSingleton(sp => ActivatorUtilities.CreateInstance<TelegramBotMessageHandler>(sp, options.ConnectToMapLinkTemplate));

        if (!string.IsNullOrWhiteSpace(options.AccessToken))
        {
            services
                .AddHostedService(sp => ActivatorUtilities.CreateInstance<TelegramBotConnector>(sp, options.AccessToken));
        }

        return services;
    }
}