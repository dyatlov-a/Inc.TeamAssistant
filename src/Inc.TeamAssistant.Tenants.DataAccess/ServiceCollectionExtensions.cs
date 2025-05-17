using Inc.TeamAssistant.Tenants.Application.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.Tenants.DataAccess;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTenantsDataAccess(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddSingleton<ITenantRepository, TenantRepository>()
            .AddSingleton<ITenantReader, TenantReader>();

        return services;
    }
}