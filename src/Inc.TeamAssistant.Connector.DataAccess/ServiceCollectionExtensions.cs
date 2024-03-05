using Dapper;
using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.Connector.DataAccess;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConnectorDataAccess(this IServiceCollection services, string connectionString)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));
        
        SqlMapper.AddTypeHandler(new MessageIdTypeHandler());
        SqlMapper.AddTypeHandler(new LanguageIdTypeHandler());
        
        services
            .AddSingleton<ITeamRepository>(
                sp => ActivatorUtilities.CreateInstance<TeamRepository>(sp, connectionString))
            .AddSingleton<IPersonRepository>(
                sp => ActivatorUtilities.CreateInstance<PersonRepository>(sp, connectionString))
            .AddSingleton<IBotRepository>(
                sp => ActivatorUtilities.CreateInstance<BotRepository>(sp, connectionString))
            .AddSingleton<ITeamAccessor>(
                sp => ActivatorUtilities.CreateInstance<TeamAccessor>(sp, connectionString));
        
        return services;
    }
}