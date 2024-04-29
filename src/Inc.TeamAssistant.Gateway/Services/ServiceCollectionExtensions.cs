using FluentValidation;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Gateway.Services.Clients;
using Inc.TeamAssistant.Gateway.Services.Internal;
using Inc.TeamAssistant.Gateway.Services.Render;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.WebUI.Contracts;
using Prometheus.DotNetRuntime;

namespace Inc.TeamAssistant.Gateway.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureMetrics(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        
        DotNetRuntimeStatsBuilder
            .Customize()
            .WithGcStats()
            .WithThreadPoolStats()
            .StartCollecting();
        
        return services;
    }
    
    public static IServiceCollection ConfigureValidator(this IServiceCollection services, LanguageId languageId)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(languageId);

        ValidatorOptions.Global.LanguageManager.Culture = new(languageId.Value);
        ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Continue;
        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
        
        return services;
    }
    
	public static IServiceCollection AddServices(
        this IServiceCollection services,
        string webRootPath,
        TimeSpan cacheAbsoluteExpiration)
	{
        ArgumentNullException.ThrowIfNull(services);
        
        if (string.IsNullOrWhiteSpace(webRootPath))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(webRootPath));

        services
            .AddSingleton(new MessageProvider(webRootPath))
            .AddSingleton<IMessageProvider>(sp => ActivatorUtilities.CreateInstance<MessageProviderCached>(
                sp,
                sp.GetRequiredService<MessageProvider>(),
                cacheAbsoluteExpiration));

        services
            .AddScoped<IAppraiserService, AppraiserService>()
            .AddScoped<IMessagesSender, MessagesSender>()
            .AddScoped<ICheckInService, CheckInService>()

            .AddSingleton<QuickResponseCodeGenerator>()
            .AddSingleton<IQuickResponseCodeGenerator>(sp => ActivatorUtilities.CreateInstance<QuickResponseCodeGeneratorCached>(
                sp,
                sp.GetRequiredService<QuickResponseCodeGenerator>(),
                cacheAbsoluteExpiration))

            .AddSingleton<IMessageBuilder, MessageBuilder>()
            .AddSingleton<ILinkBuilder, LinkBuilder>()
            
            .AddScoped<IRenderContext, ServerRenderContext>();

        return services;
	}
}