using Blazored.LocalStorage;
using FluentValidation;
using Inc.TeamAssistant.WebUI.Contracts;
using Inc.TeamAssistant.WebUI.Features.Components;
using Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage3;
using Inc.TeamAssistant.WebUI.Services.ClientCore;
using Inc.TeamAssistant.WebUI.Services.Clients;
using Microsoft.AspNetCore.Components.Authorization;

namespace Inc.TeamAssistant.WebUI.Services;

public static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        var appVersion = ApplicationContext.GetVersion();
        
        services
            .AddBlazoredLocalStorage()
            
            .AddScoped<IAppraiserService, AppraiserClient>()
            .AddScoped<ICheckInService, CheckInClient>()
            .AddScoped<IUserService, UserClient>()
            .AddScoped<IBotService, BotClient>()
            .AddScoped<IReviewService, ReviewClient>()
            .AddScoped<IRandomCoffeeService, RandomCoffeeClient>()
            .AddScoped<ICalendarService, CalendarClient>()
            .AddScoped<IIntegrationService, IntegrationClient>()
            .AddScoped(sp => ActivatorUtilities.CreateInstance<DataEditor>(sp, appVersion))
            .AddScoped<IValidator<BotDetailsItemFormModel>, BotDetailsItemFormModelValidator>()
            
            .AddSingleton<IRenderContext, ClientRenderContext>()
            .AddSingleton<IMessageProvider, MessageProviderClient>()
            
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
            .AddScoped<ResourcesManager>()
            .AddScoped<DateSelectorFactory>()
            .AddScoped<RequestProcessor>()
            .AddSingleton<LinkBuilder>()
            .AddScoped(typeof(DragAndDropService<>));

        return services;
    }
}