using System.Reflection;
using Blazored.LocalStorage;
using Inc.TeamAssistant.WebUI.Services.CheckIn;
using Inc.TeamAssistant.Appraiser.Model;
using Inc.TeamAssistant.CheckIn.Model;
using Inc.TeamAssistant.Common.Messages;
using Inc.TeamAssistant.Common.Messages.Client;

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

            .AddScoped<IAssessmentSessionsService, AssessmentSessionsClient>()
            .AddScoped<IClientInfoService, ClientInfoClient>()
            .AddSingleton<IEventsProvider, EventsProviderClient>()
            .AddSingleton<ICookieService, CookieServiceClient>()
            .AddScoped<ILocationBuilder, LocationBuilder>()

            .AddSingleton<MessageServiceClient>()
            .AddSingleton<IMessageService>(sp => new MessageServiceCached(
                sp.GetRequiredService<ILocalStorageService>(),
                sp.GetRequiredService<MessageServiceClient>(),
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