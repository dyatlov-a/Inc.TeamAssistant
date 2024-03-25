using Dapper;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.Appraiser.DataAccess;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppraiserDataAccess(this IServiceCollection services, string connectionString)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));
        
        SqlMapper.AddTypeHandler(new JsonTypeHandler<ICollection<string>>());
        SqlMapper.AddTypeHandler(new JsonTypeHandler<IReadOnlyDictionary<string, string>>());
        SqlMapper.AddTypeHandler(new LanguageIdTypeHandler());
        
        services
            .AddSingleton<IStoryReader>(sp => ActivatorUtilities.CreateInstance<StoryReader>(sp, connectionString))
            .AddSingleton<IStoryRepository>(
                sp => ActivatorUtilities.CreateInstance<StoryRepository>(sp, connectionString));

        return services;
    }
}