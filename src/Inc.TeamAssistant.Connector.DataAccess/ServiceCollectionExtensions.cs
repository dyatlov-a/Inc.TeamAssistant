using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.Connector.DataAccess;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConnectorDataAccess(this IServiceCollection services, TimeSpan cacheTimeout)
    {
        ArgumentNullException.ThrowIfNull(services);
        
        services
            .AddSingleton<IPhotosRepository, PhotosRepository>()
            .AddSingleton<ITeamRepository, TeamRepository>()
            .AddSingleton<ITeamAccessor, TeamAccessor>()
            
            .AddSingleton<IBotReader, BotReader>()
            
            .AddSingleton<PersonRepository>()
            .AddSingleton<IPersonRepository>(sp => ActivatorUtilities.CreateInstance<CachedPersonRepository>(
                sp,
                sp.GetRequiredService<PersonRepository>(),
                cacheTimeout))
            
            .AddSingleton<ClientLanguageRepository>()
            .AddSingleton<IClientLanguageRepository>(sp => ActivatorUtilities.CreateInstance<CachedClientLanguageRepository>(
                sp,
                sp.GetRequiredService<ClientLanguageRepository>(),
                cacheTimeout));
        
        return services;
    }
}