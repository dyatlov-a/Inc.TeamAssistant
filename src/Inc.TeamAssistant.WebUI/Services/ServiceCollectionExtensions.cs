using Blazored.LocalStorage;
using Inc.TeamAssistant.WebUI.Contracts;
using Inc.TeamAssistant.WebUI.Services.Clients;
using Inc.TeamAssistant.WebUI.Services.Core;
using Inc.TeamAssistant.WebUI.Services.Render;
using Microsoft.AspNetCore.Components.Authorization;

namespace Inc.TeamAssistant.WebUI.Services;

public static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddBlazoredLocalStorage()
            
            .AddScoped<IAppraiserService, AppraiserClient>()
            .AddScoped<ICheckInService, CheckInClient>()
            .AddScoped<IUserService, UserClient>()
            .AddScoped<IBotService, BotClient>()
            
            .AddScoped<IRenderContext, ClientRenderContext>()
            .AddSingleton<MessageProviderClient>()
            .AddSingleton<IMessageProvider>(sp => new MessageProviderClientCached(
                sp.GetRequiredService<ILocalStorageService>(),
                sp.GetRequiredService<MessageProviderClient>(),
                ApplicationContext.GetVersion()))
            
            .AddTransient<EventsProvider>();

        return services;
    }

    public static IServiceCollection AddIsomorphic(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddOptions()
            .AddAuthorizationCore()
            .AddScoped<AuthenticationStateProvider, AuthStateProvider>()
            .AddScoped<LanguageManager>();

        return services;
    }
}