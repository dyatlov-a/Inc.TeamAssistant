using Inc.TeamAssistant.CheckIn.Application.Services;
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
            .AddSingleton(sp => ActivatorUtilities.CreateInstance<TelegramBotMessageHandler>(
                sp,
                options.ConnectToMapLinkTemplate));

        if (!string.IsNullOrWhiteSpace(options.AccessToken))
        {
            services
                .AddHostedService(sp => ActivatorUtilities.CreateInstance<TelegramBotConnector>(
                    sp,
                    options.AccessToken));
        }

        return services;
    }
}