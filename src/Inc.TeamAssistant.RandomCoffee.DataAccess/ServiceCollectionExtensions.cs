using Dapper;
using Inc.TeamAssistant.RandomCoffee.Application.Contracts;
using Inc.TeamAssistant.RandomCoffee.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.RandomCoffee.DataAccess;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRandomCoffeeDataAccess(
        this IServiceCollection services,
        string connectionString)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));

        SqlMapper.AddTypeHandler(new JsonTypeHandler<ICollection<PersonPair>>());
        SqlMapper.AddTypeHandler(new JsonTypeHandler<ICollection<long>>());
        
        services
            .AddSingleton<IRandomCoffeeReader>(sp => ActivatorUtilities.CreateInstance<RandomCoffeeReader>(
                sp,
                connectionString))
                
            .AddSingleton<IRandomCoffeeRepository>(sp => ActivatorUtilities.CreateInstance<RandomCoffeeRepository>(
                sp,
                connectionString));

        return services;
    }
}