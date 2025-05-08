using Inc.TeamAssistant.Retro.Application.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.Retro.DataAccess;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRetroDataAccess(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddSingleton<IRetroRepository, RetroRepository>();

        return services;
    }
}