using Inc.TeamAssistant.Primitives.Features.PersonStats;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.Reviewer.DataAccess;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddReviewerDataAccess(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddSingleton<ITaskForReviewRepository, TaskForReviewRepository>()
            .AddSingleton<ITaskForReviewReader, TaskForReviewReader>()
            .AddSingleton<IReviewAnalyticsReader, ReviewAnalyticsReader>()
            .AddSingleton<IDraftTaskForReviewRepository, DraftTaskForReviewRepository>()
            .AddSingleton<ITeammateRepository, TeammateRepository>()
            
            .AddSingleton<IPersonStatsProvider, ReviewStatsProvider>();

        return services;
    }
}