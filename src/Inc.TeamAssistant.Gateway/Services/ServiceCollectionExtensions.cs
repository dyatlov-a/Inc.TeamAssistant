using Inc.TeamAssistant.Gateway.Configs;
using Inc.TeamAssistant.Gateway.Services.Clients;
using Inc.TeamAssistant.Gateway.Services.InMemory;
using Inc.TeamAssistant.Gateway.Services.Integrations;
using Inc.TeamAssistant.Gateway.Services.ServerCore;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Survey.Application.Contracts;
using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Inc.TeamAssistant.Gateway.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(
        this IServiceCollection services,
        OpenGraphOptions openGraphOptions,
        string webRootPath,
        TimeSpan cacheTimeout)
	{
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(openGraphOptions);
        ArgumentException.ThrowIfNullOrWhiteSpace(webRootPath);
        
        services.TryAddScoped<IWebAssemblyHostEnvironment, ServerHostEnvironment>();

        services
            .AddQuickResponseCodeGenerator(cacheTimeout)
                
            .AddSingleton(openGraphOptions)
            .AddSingleton(MessageDataBuilder.Build(webRootPath))
            .AddSingleton<IRenderContext, ServerRenderContext>()
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
            .AddScoped<ISurveyService, SurveyService>()
            .AddScoped<IIntegrationService, IntegrationService>()
            .AddSingleton(sp => ActivatorUtilities.CreateInstance<OpenGraphService>(sp, webRootPath))

            .AddSingleton<IMessageBuilder, MessageBuilder>()
            .AddSingleton<ITeamLinkBuilder, TeamLinkBuilder>()
            
            .AddSingleton<IOnlinePersonStore, OnlinePersonInMemoryStore>()
            .AddSingleton<IPositionGenerator, PositionInMemoryGenerator>()
            .AddSingleton<ITimerService, TimerInMemoryService>()
            .AddSingleton<IVoteStore, VoteInMemoryStore>()
            .AddSingleton<IRetroStage, RetroStageInMemory>()
            .AddSingleton<ISurveyState, SurveyState>();

        return services;
	}

    private static IServiceCollection AddQuickResponseCodeGenerator(
        this IServiceCollection services,
        TimeSpan cacheTimeout)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddSingleton<QuickResponseCodeGenerator>()
            .AddSingleton<IQuickResponseCodeGenerator>(sp =>
                ActivatorUtilities.CreateInstance<QuickResponseCodeGeneratorCached>(
                    sp,
                    sp.GetRequiredService<QuickResponseCodeGenerator>(),
                    cacheTimeout));
        
        return services;
    }
}