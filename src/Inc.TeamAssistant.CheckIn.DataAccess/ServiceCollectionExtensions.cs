using Inc.TeamAssistant.CheckIn.Application.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.CheckIn.DataAccess;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCheckInDataAccess(
        this IServiceCollection services,
        string connectionString)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));

        services
            .AddSingleton<ILocationsRepository>(sp => ActivatorUtilities.CreateInstance<LocationsRepository>(
                sp,
                connectionString));

        return services;
    }
}