using Inc.TeamAssistant.Tenants.Application.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.Tenants.DataAccess;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTenantsDataAccess(this IServiceCollection services, TimeSpan cacheTimeout)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddSingleton<ITenantRepository, TenantRepository>()
            .AddSingleton<ITenantReader, TenantReader>()

            .AddSingleton<RoomPropertiesProvider>()
            .AddSingleton<IRoomPropertiesProvider>(sp =>
                ActivatorUtilities.CreateInstance<CachedRoomPropertiesProvider>(
                    sp,
                    sp.GetRequiredService<RoomPropertiesProvider>(),
                    cacheTimeout));

        return services;
    }
}