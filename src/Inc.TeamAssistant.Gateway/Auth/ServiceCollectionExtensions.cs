using Inc.TeamAssistant.Gateway.Configs;

namespace Inc.TeamAssistant.Gateway.Auth;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthServices(this IServiceCollection services, AuthOptions authOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(authOptions);

        services
            .AddSingleton(authOptions)
            .AddScoped<TelegramAuthService>();
        
        return services;
    }
}