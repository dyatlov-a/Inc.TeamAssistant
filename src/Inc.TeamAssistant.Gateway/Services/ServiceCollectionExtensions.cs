using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model;
using Inc.TeamAssistant.Gateway.Services.MessageProviders;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Gateway.Services;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddServices(
        this IServiceCollection services,
        string webRootPath,
        TimeSpan cacheAbsoluteExpiration)
	{
		if (services is null)
			throw new ArgumentNullException(nameof(services));
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
            .AddSingleton<IEventsProvider, EventsProvider>()
            .AddScoped<ICookieService, CookieService>()
            .AddScoped<IMessagesSender, MessagesSender>()
            .AddScoped<IClientInfoService, ClientInfoService>()

            .AddSingleton<QuickResponseCodeGenerator>()
            .AddSingleton<IQuickResponseCodeGenerator>(sp => ActivatorUtilities.CreateInstance<QuickResponseCodeGeneratorCached>(
                sp,
                sp.GetRequiredService<QuickResponseCodeGenerator>(),
                cacheAbsoluteExpiration))

            .AddSingleton<IMessageBuilder, MessageBuilder>()
            .AddSingleton<ILinkBuilder, LinkBuilder>();

        return services;
	}
}