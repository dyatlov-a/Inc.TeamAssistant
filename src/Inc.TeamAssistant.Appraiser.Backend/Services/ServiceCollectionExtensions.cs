using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Backend.Services.CommandFactories;
using Inc.TeamAssistant.Appraiser.Model;
using Inc.TeamAssistant.Appraiser.Notifications.Contracts;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Appraiser.Backend.Services;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddServices(this IServiceCollection services, TelegramBotOptions options)
	{
		if (services is null)
			throw new ArgumentNullException(nameof(services));
        if (options is null)
            throw new ArgumentNullException(nameof(options));

        foreach (var languageId in Settings.LanguageIds)
            services.AddSingleton(new LanguageContext(languageId, string.Format(CommandList.ChangeLanguageForAssessmentSession, languageId.Value)));

        services
            .AddSingleton<ICommandProvider, CommandProvider>()
			.AddSingleton<StaticCommandFactory>()
			.AddScoped<DynamicCommandFactory>()
			.AddScoped<ICommandFactory, ComplexCommandFactory>();

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
                options.CacheAbsoluteExpiration));

        return services;
	}
}