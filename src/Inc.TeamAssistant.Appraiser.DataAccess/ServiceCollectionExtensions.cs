using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.Appraiser.DataAccess;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppraiserDataAccess(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddSingleton<IStoryReader, StoryReader>()
            .AddSingleton<IStoryRepository, StoryRepository>()
            
            .AddSingleton<IPersonStatsProvider, EstimatesStatsProvider>();

        return services;
    }
}