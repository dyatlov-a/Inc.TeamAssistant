using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.Appraiser.DataAccess;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppraiserDataAccess(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddSingleton<IStoryReader, StoryReader>()
            .AddSingleton<IStoryRepository, StoryRepository>();

        return services;
    }
}