using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Integrations;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.Connector.DataAccess;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConnectorDataAccess(this IServiceCollection services, TimeSpan cacheTimeout)
    {
        ArgumentNullException.ThrowIfNull(services);
        
        services
            .AddSingleton<IIntegrationsAccessor, IntegrationsAccessor>()
            .AddSingleton<ITeamAccessor, TeamAccessor>()
            
            .AddSingleton<IBotReader, BotReader>()
            .AddSingleton<ITeamReader, TeamReader>()
            
            .AddSingleton<ITeamRepository, TeamRepository>()
            .AddSingleton<IDashboardSettingsRepository, DashboardSettingsRepository>()
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