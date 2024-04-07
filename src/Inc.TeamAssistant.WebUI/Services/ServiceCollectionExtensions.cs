using System.Reflection;
using Blazored.LocalStorage;
using Inc.TeamAssistant.WebUI.Services.CheckIn;
using Inc.TeamAssistant.Appraiser.Model;
using Inc.TeamAssistant.CheckIn.Model;

namespace Inc.TeamAssistant.WebUI.Services;

public static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddServices(this IServiceCollection services)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        var appVersion = Assembly.GetExecutingAssembly().GetName().Version!.ToString();

        services
            .AddBlazoredLocalStorage()

            .AddScoped<IAppraiserService, AppraiserClient>()
            .AddScoped<IClientInfoService, ClientInfoClient>()
            .AddTransient<IEventsProvider, EventsProviderClient>()
            .AddSingleton<ICookieService, CookieServiceClient>()
            .AddScoped<ICheckInService, CheckInClient>()
            .AddScoped<ILocationBuilder, LocationBuilder>()

            .AddSingleton<MessageProviderClient>()
            .AddSingleton<IMessageProvider>(sp => new MessageProviderClientCached(
                sp.GetRequiredService<ILocalStorageService>(),
                sp.GetRequiredService<MessageProviderClient>(),
                appVersion));

        return services;
    }

    public static IServiceCollection AddIsomorphic(this IServiceCollection services)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        services
            .AddScoped<LanguageManager>();

        return services;
    }
}