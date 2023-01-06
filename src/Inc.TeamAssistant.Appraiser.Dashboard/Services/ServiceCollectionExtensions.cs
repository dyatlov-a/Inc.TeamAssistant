using System.Reflection;
using Blazored.LocalStorage;
using Inc.TeamAssistant.Appraiser.Dashboard.Services.CheckIn;
using Inc.TeamAssistant.Appraiser.Model;
using Inc.TeamAssistant.Appraiser.Model.CheckIn;

namespace Inc.TeamAssistant.Appraiser.Dashboard.Services;

public static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddServices(this IServiceCollection services)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        var appVersion = Assembly.GetExecutingAssembly().GetName().Version!.ToString();

        services
            .AddBlazoredLocalStorage()

            .AddScoped<IAssessmentSessionsService, AssessmentSessionsClient>()
            .AddScoped<IClientInfoService, ClientInfoClient>()
            .AddSingleton<IEventsProvider, EventsProviderClient>()
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
            .AddScoped<ISwipeService, SwipeService>()
            .AddScoped<LanguageManager>();

        return services;
    }
}