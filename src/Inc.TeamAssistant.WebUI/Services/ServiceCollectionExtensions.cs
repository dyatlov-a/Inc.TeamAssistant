using Blazored.LocalStorage;
using FluentValidation;
using Inc.TeamAssistant.WebUI.Components;
using Inc.TeamAssistant.WebUI.Components.Notifications;
using Inc.TeamAssistant.WebUI.Contracts;
using Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage1;
using Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage2;
using Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage3;
using Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage4;
using Inc.TeamAssistant.WebUI.Features.Dashboard.Appraiser;
using Inc.TeamAssistant.WebUI.Features.Dashboard.Settings;
using Inc.TeamAssistant.WebUI.Features.Layouts;
using Inc.TeamAssistant.WebUI.Features.Tenants;
using Inc.TeamAssistant.WebUI.Routing;
using Inc.TeamAssistant.WebUI.Services.Internal;
using Inc.TeamAssistant.WebUI.Services.ServiceClients;
using Inc.TeamAssistant.WebUI.Services.Stores;

namespace Inc.TeamAssistant.WebUI.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddClientServices(this IServiceCollection services)
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
            .AddScoped<ITenantService, TenantClient>()
            .AddScoped<IRetroService, RetroClient>()
            .AddScoped<IIntegrationService, IntegrationClient>()
            .AddScoped(sp => ActivatorUtilities.CreateInstance<AppLocalStorage>(sp, appVersion))
            .AddSingleton<IRenderContext, ClientRenderContext>()
            .AddSingleton<EventsProvider>()
            .AddNotificationsService(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(5))

            .AddAuthorizationCore()
            .AddCascadingAuthenticationState()
            .AddAuthenticationStateDeserialization();

        return services;
    }

    public static IServiceCollection AddIsomorphicServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddOptions()
            .AddLocalization()
            .AddScoped<FeaturesFactory>()
            .AddScoped<IDateSelectorFactory, DateSelectorFactory>()
            .AddScoped<MainFooterLinksFactory>()
            .AddScoped<RequestProcessor>()
            .AddScoped<NavRouter>()
            .AddScoped(typeof(DragAndDropService<>))
            .AddScoped<TenantStore>()
            
            .AddScoped<IValidator<CheckBotFormModel>, CheckBotFormModelValidator>()
            .AddScoped<IValidator<SelectFeaturesFormModel>, SelectFeaturesFormValidator>()
            .AddScoped<IValidator<BotDetailsFormModel>, BotDetailsFormModelValidator>()
            .AddScoped<IValidator<BotDetailsItemFormModel>, BotDetailsItemFormModelValidator>()
            .AddScoped<IValidator<CalendarFormModel>, CalendarFromModelValidator>()
            .AddScoped<IValidator<SettingsFormModel>, SettingsFormModelValidator>()
            .AddScoped<IValidator<CompleteFormModel>, CompleteFormModelValidator>()
            .AddScoped<IValidator<AppraiserIntegrationFromModel>, AppraiserIntegrationFromModelValidator>()
            .AddScoped<IValidator<DashboardSettingsFormModel>, DashboardSettingsFormModelValidator>()
            .AddScoped<IValidator<TenantTeamFormModel>, TenantTeamFormModelValidator>();

        return services;
    }
    
    private static IServiceCollection AddNotificationsService(
        this IServiceCollection services,
        TimeSpan messageLifetime,
        TimeSpan checkInterval)
    {
        ArgumentNullException.ThrowIfNull(services);
        
        var notificationsService = new NotificationsService(messageLifetime, checkInterval);
        
        services
            .AddSingleton<INotificationsSource>(notificationsService)
            .AddSingleton<INotificationsService>(notificationsService);
        
        return services;
    }
}