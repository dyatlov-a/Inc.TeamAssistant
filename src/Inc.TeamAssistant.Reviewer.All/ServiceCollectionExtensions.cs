using Dapper;
using Inc.TeamAssistant.Reviewer.All.Contracts;
using Inc.TeamAssistant.Reviewer.All.Internal;
using Inc.TeamAssistant.Reviewer.All.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.Reviewer.All;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddReviewer(
        this IServiceCollection services,
        ReviewerOptions options,
        string connectionString)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));
        if (options is null)
            throw new ArgumentNullException(nameof(options));
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));

        SqlMapper.AddTypeHandler(new LanguageIdTypeHandler());

        services
            .AddSingleton<ITaskForReviewRepository>(
                sp => ActivatorUtilities.CreateInstance<TaskForReviewRepository>(sp, connectionString))
            .AddSingleton<ITeamRepository>(
                sp => ActivatorUtilities.CreateInstance<TeamRepository>(sp, connectionString))
            .AddSingleton<ITaskForReviewAccessor>(
                sp => ActivatorUtilities.CreateInstance<TaskForReviewAccessor>(sp, connectionString))

            .AddSingleton(sp => ActivatorUtilities.CreateInstance<TelegramBotMessageHandler>(
                sp,
                options.BotLink,
                options.LinkForConnectTemplate,
                options.BotName,
                options.Workday.NotificationInterval));

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