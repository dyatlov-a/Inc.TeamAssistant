using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.RandomCoffee.Application.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.RandomCoffee.DataAccess;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRandomCoffeeDataAccess(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        
        services
            .AddSingleton<IRandomCoffeeReader, RandomCoffeeReader>()
            .AddSingleton<IRandomCoffeeRepository, RandomCoffeeRepository>()
            
            .AddSingleton<IPersonStatsProvider, RandomCoffeeStatsProvider>();

        return services;
    }
}