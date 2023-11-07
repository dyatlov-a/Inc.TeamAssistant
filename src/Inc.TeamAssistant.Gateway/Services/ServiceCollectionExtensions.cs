using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model;
using Inc.TeamAssistant.Gateway.Services.CommandFactories;
using Inc.TeamAssistant.Gateway.Services.MessageProviders;
using Inc.TeamAssistant.Languages;

namespace Inc.TeamAssistant.Gateway.Services;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddServices(
        this IServiceCollection services,
        TelegramBotOptions options,
        string webRootPath)
	{
		if (services is null)
			throw new ArgumentNullException(nameof(services));
        if (options is null)
            throw new ArgumentNullException(nameof(options));
        if (string.IsNullOrWhiteSpace(webRootPath))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(webRootPath));

        foreach (var languageId in LanguageSettings.LanguageIds)
            services.AddSingleton(new LanguageContext(languageId, string.Format(CommandList.ChangeLanguageForAssessmentSession, languageId.Value)));

        services
            .AddMemoryCache()
            .AddHttpContextAccessor();

        services
            .AddSingleton<ICommandProvider, CommandProvider>()
			.AddSingleton<StaticCommandFactory>()
			.AddScoped<DynamicCommandFactory>()
			.AddScoped<ICommandFactory, ComplexCommandFactory>();

        services
            .AddSingleton(new MessageProvider(webRootPath))
            .AddSingleton<IMessageProvider>(sp => ActivatorUtilities.CreateInstance<MessageProviderCached>(
                sp,
                sp.GetRequiredService<MessageProvider>(),
                options.CacheAbsoluteExpiration));

        if (!string.IsNullOrWhiteSpace(options.AccessToken))
        {
            services
                .AddHostedService(sp => ActivatorUtilities.CreateInstance<TelegramBotConnector>(sp, options.AccessToken));
        }

        services
            .AddSingleton<TelegramBotMessageHandler>()

            .AddScoped<IAssessmentSessionsService, AssessmentSessionsService>()
            .AddSingleton<IEventsProvider, EventsProvider>()
            .AddScoped<ICookieService, CookieService>()
            .AddScoped<IMessagesSender, MessagesSender>()
            .AddSingleton<IAssessmentSessionMetrics, AssessmentSessionMetrics>()
            .AddScoped<IClientInfoService, ClientInfoService>()
            .AddScoped<ILinkBuilder>(sp => ActivatorUtilities.CreateInstance<LinkBuilder>(
                sp,
                options.Link,
                options.ConnectToSessionLinkTemplate,
                options.ConnectToDashboardLinkTemplate))

            .AddSingleton<QuickResponseCodeGenerator>()
            .AddSingleton<IQuickResponseCodeGenerator>(sp => ActivatorUtilities.CreateInstance<QuickResponseCodeGeneratorCached>(
                sp,
                sp.GetRequiredService<QuickResponseCodeGenerator>(),
                options.CacheAbsoluteExpiration))

            .AddScoped<IMessageBuilder, MessageBuilder>();

        return services;
	}
}