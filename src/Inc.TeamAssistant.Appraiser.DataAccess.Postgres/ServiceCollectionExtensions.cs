using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Inc.TeamAssistant.Appraiser.DataAccess.Postgres;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPostgresDataAccess(
        this IServiceCollection services,
        string connectionString,
        string anonymousUser)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));
        if (string.IsNullOrWhiteSpace(anonymousUser))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(anonymousUser));

        services
            .AddSingleton(_ => new UserSettingsProvider(connectionString, anonymousUser))
            .AddSingleton<IUserSettingsProvider>(
                sp => new UserSettingsProviderFailTolerance(
                    sp.GetRequiredService<UserSettingsProvider>(),
                    sp.GetRequiredService<ILogger<UserSettingsProviderFailTolerance>>(),
                    anonymousUser));

        return services;
    }
}