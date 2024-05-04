using Inc.TeamAssistant.Constructor.Application.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.Constructor.DataAccess;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConstructorDataAccess(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        
        services
            .AddSingleton<IBotRepository, BotRepository>();
        
        return services;
    }
}