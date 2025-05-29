using Inc.TeamAssistant.Gateway.Configs;
using Inc.TeamAssistant.Gateway.Services.Clients;
using Inc.TeamAssistant.Gateway.Services.Integrations;
using Inc.TeamAssistant.Gateway.Services.ServerCore;
using Inc.TeamAssistant.Gateway.Services.Stores;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Inc.TeamAssistant.Gateway.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(
        this IServiceCollection services,
        AuthOptions authOptions,
        OpenGraphOptions openGraphOptions,
        string webRootPath,
        TimeSpan cacheTimeout)
	{
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(authOptions);
        ArgumentNullException.ThrowIfNull(openGraphOptions);
        ArgumentException.ThrowIfNullOrWhiteSpace(webRootPath);
        
        services
            .TryAddScoped<IWebAssemblyHostEnvironment, ServerHostEnvironment>();

        services
            .AddSingleton(authOptions)
            .AddSingleton(openGraphOptions)
            .AddSingleton(MessageDataBuilder.Build(webRootPath))
            .AddSingleton<IRenderContext, ServerRenderContext>()
            .AddSingleton<IPositionGenerator, PositionGenerator>()
            .AddScoped<TelegramAuthService>()
            .AddScoped<EstimatesService>()
            .AddScoped<IntegrationContextProvider>()
            .AddScoped<IAppraiserService, AppraiserService>()
            .AddScoped<ICheckInService, CheckInService>()
            .AddScoped<IUserService, UserService>()
            .AddScoped<IBotService, BotService>()
            .AddScoped<IPersonResolver, PersonResolver>()
            .AddScoped<IReviewService, ReviewService>()
            .AddScoped<IRandomCoffeeService, RandomCoffeeService>()
            .AddScoped<ICalendarService, CalendarService>()
            .AddScoped<ITenantService, TenantService>()
            .AddScoped<IRetroService, RetroService>()
            .AddScoped<IIntegrationService, IntegrationService>()
            .AddSingleton(sp => ActivatorUtilities.CreateInstance<OpenGraphService>(sp, webRootPath))
            .AddSingleton<QuickResponseCodeGenerator>()
            .AddSingleton<IQuickResponseCodeGenerator>(sp =>
                ActivatorUtilities.CreateInstance<QuickResponseCodeGeneratorCached>(
                    sp,
                    sp.GetRequiredService<QuickResponseCodeGenerator>(),
                    cacheTimeout))

            .AddSingleton<IMessageBuilder, MessageBuilder>()
            .AddSingleton<ITeamLinkBuilder, TeamLinkBuilder>()
            
            .AddSingleton<IOnlinePersonStore, OnlinePersonInMemoryStore>()
            .AddSingleton<IVoteStore, VoteInMemoryStore>();

        return services;
	}
}