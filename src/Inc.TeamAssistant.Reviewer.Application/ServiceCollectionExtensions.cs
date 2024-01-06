using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.Reviewer.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddReviewerApplication(this IServiceCollection services, ReviewerOptions options)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));
        if (options is null)
            throw new ArgumentNullException(nameof(options));

        services
            .AddSingleton(options)
            .AddScoped<IMessageBuilderService, MessageBuilderService>()
            .AddSingleton(sp => ActivatorUtilities.CreateInstance<TelegramBotMessageHandler>(
                sp,
                options.BotLink,
                options.LinkForConnectTemplate,
                options.BotName));

        if (!string.IsNullOrWhiteSpace(options.AccessToken))
        {
            services
                .AddHostedService(
                    sp => ActivatorUtilities.CreateInstance<NotificationsService>(
                        sp,
                        options.Workday,
                        options.AccessToken,
                        options.NotificationsBatch,
                        options.NotificationsDelay))
                .AddHostedService(
                    sp => ActivatorUtilities.CreateInstance<TelegramBotConnector>(sp, options.AccessToken));
        }

        return services;
    }
}