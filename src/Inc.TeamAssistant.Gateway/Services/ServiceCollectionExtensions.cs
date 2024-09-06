using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Gateway.Configs;
using Inc.TeamAssistant.Gateway.Services.Clients;
using Inc.TeamAssistant.Gateway.Services.Integrations;
using Inc.TeamAssistant.Gateway.Services.ServerCore;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.Gateway.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(
        this IServiceCollection services,
        AuthOptions authOptions,
        OpenGraphOptions openGraphOptions,
        string webRootPath,
        TimeSpan cacheAbsoluteExpiration)
	{
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(authOptions);
        ArgumentNullException.ThrowIfNull(openGraphOptions);
        ArgumentException.ThrowIfNullOrWhiteSpace(webRootPath);

        services
            .AddSingleton(authOptions)
            .AddSingleton(openGraphOptions)
            .AddSingleton(new MessageProvider(webRootPath))
            .AddSingleton<IMessageProvider>(sp => ActivatorUtilities.CreateInstance<MessageProviderCached>(
                sp,
                sp.GetRequiredService<MessageProvider>(),
                cacheAbsoluteExpiration))
            .AddSingleton<IRenderContext, ServerRenderContext>()
            .AddScoped<TelegramAuthService>()
            .AddScoped<EstimatesService>()
            .AddScoped<IntegrationContextProvider>()
            .AddScoped<IAppraiserService, AppraiserService>()
            .AddScoped<IMessagesSender, MessagesSender>()
            .AddScoped<ICheckInService, CheckInService>()
            .AddScoped<IUserService, UserService>()
            .AddScoped<IBotService, BotService>()
            .AddScoped<ICurrentPersonResolver, CurrentPersonResolver>()
            .AddScoped<IReviewService, ReviewService>()
            .AddScoped<IRandomCoffeeService, RandomCoffeeService>()
            .AddScoped<ICalendarService, CalendarService>()
            .AddScoped<IIntegrationService, IntegrationService>()
            .AddSingleton(sp => ActivatorUtilities.CreateInstance<OpenGraphService>(sp, webRootPath))
            
            .AddSingleton<QuickResponseCodeGenerator>()
            .AddSingleton<IQuickResponseCodeGenerator>(sp => ActivatorUtilities.CreateInstance<QuickResponseCodeGeneratorCached>(
                sp,
                sp.GetRequiredService<QuickResponseCodeGenerator>(),
                cacheAbsoluteExpiration))

            .AddSingleton<IMessageBuilder, MessageBuilder>()
            .AddSingleton<ITeamLinkBuilder, TeamLinkBuilder>();

        return services;
	}
}