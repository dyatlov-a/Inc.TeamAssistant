using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.Reviewer.DataAccess;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddReviewerDataAccess(this IServiceCollection services, string connectionString)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));

        services
            .AddSingleton<ITaskForReviewRepository>(sp => ActivatorUtilities.CreateInstance<TaskForReviewRepository>(
                sp,
                connectionString))
            .AddSingleton<ITaskForReviewAccessor>(sp => ActivatorUtilities.CreateInstance<TaskForReviewAccessor>(
                sp,
                connectionString));

        return services;
    }
}