using System.Reflection;
using Blazored.LocalStorage;
using Inc.TeamAssistant.Appraiser.Model;
using Inc.TeamAssistant.CheckIn.Model;
using Inc.TeamAssistant.WebUI.Services.Clients;
using Inc.TeamAssistant.WebUI.Services.Render;

namespace Inc.TeamAssistant.WebUI.Services;

public static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        var appVersion = Assembly.GetExecutingAssembly().GetName().Version!.ToString();

        services
            .AddBlazoredLocalStorage()

            .AddScoped<IAppraiserService, AppraiserClient>()
            .AddScoped<ILanguageProvider, LanguageProviderClient>()
            .AddTransient<IEventsProvider, EventsProviderClient>()
            .AddSingleton<ICookieService, CookieServiceClient>()
            .AddScoped<ICheckInService, CheckInClient>()
            .AddScoped<ILocationBuilder, LocationBuilder>()
            .AddSingleton<IVideoService, VideoClientService>()

            .AddSingleton<MessageProviderClient>()
            .AddSingleton<IMessageProvider>(sp => new MessageProviderClientCached(
                sp.GetRequiredService<ILocalStorageService>(),
                sp.GetRequiredService<MessageProviderClient>(),
                appVersion));

        return services;
    }

    public static IServiceCollection AddIsomorphic(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddScoped<LanguageManager>();

        return services;
    }
}