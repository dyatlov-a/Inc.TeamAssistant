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
            .AddPersonRepository(cacheTimeout)
            .AddClientLanguageRepository(cacheTimeout)
                
            .AddSingleton<IIntegrationsAccessor, IntegrationsAccessor>()
            .AddSingleton<ITeamAccessor, TeamAccessor>()
            .AddSingleton<IPersonAccessor, PersonAccessor>()
            
            .AddSingleton<IBotReader, BotReader>()
            .AddSingleton<ITeamReader, TeamReader>()
            
            .AddSingleton<ITeamRepository, TeamRepository>()
            .AddSingleton<IDashboardSettingsRepository, DashboardSettingsRepository>();
        
        return services;
    }

    private static IServiceCollection AddPersonRepository(
        this IServiceCollection services,
        TimeSpan cacheTimeout)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddSingleton<PersonRepository>()
            .AddSingleton<IPersonRepository>(sp => ActivatorUtilities.CreateInstance<CachedPersonRepository>(
                sp,
                sp.GetRequiredService<PersonRepository>(),
                cacheTimeout));

        return services;
    }
    
    private static IServiceCollection AddClientLanguageRepository(
        this IServiceCollection services,
        TimeSpan cacheTimeout)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddSingleton<ClientLanguageRepository>()
            .AddSingleton<IClientLanguageRepository>(sp =>
                ActivatorUtilities.CreateInstance<CachedClientLanguageRepository>(
                    sp,
                    sp.GetRequiredService<ClientLanguageRepository>(),
                    cacheTimeout));

        return services;
    }
}