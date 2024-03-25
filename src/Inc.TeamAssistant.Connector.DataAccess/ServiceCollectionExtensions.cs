using Dapper;
using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.Connector.DataAccess;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConnectorDataAccess(
        this IServiceCollection services,
        string connectionString,
        TimeSpan cacheTimeout)
    {
        ArgumentNullException.ThrowIfNull(services);
        
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));
        
        SqlMapper.AddTypeHandler(new MessageIdTypeHandler());
        
        services
            .AddSingleton<ITeamRepository>(
                sp => ActivatorUtilities.CreateInstance<TeamRepository>(sp, connectionString))
            .AddSingleton<IBotRepository>(
                sp => ActivatorUtilities.CreateInstance<BotRepository>(sp, connectionString))
            .AddSingleton<ITeamAccessor, TeamAccessor>()
            
            .AddSingleton(sp => ActivatorUtilities.CreateInstance<PersonRepository>(sp, connectionString))
            .AddSingleton<IPersonRepository>(sp => ActivatorUtilities.CreateInstance<CachedPersonRepository>(
                sp,
                sp.GetRequiredService<PersonRepository>(),
                cacheTimeout))
            
            .AddSingleton(sp => ActivatorUtilities.CreateInstance<ClientLanguageRepository>(sp, connectionString))
            .AddSingleton<IClientLanguageRepository>(sp => ActivatorUtilities.CreateInstance<CachedClientLanguageRepository>(
                sp,
                sp.GetRequiredService<ClientLanguageRepository>(),
                cacheTimeout));
        
        return services;
    }
}