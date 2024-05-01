using FluentValidation;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.CheckIn.Application.Contracts;
using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Gateway.Services.Clients;
using Inc.TeamAssistant.Gateway.Services.Internal;
using Inc.TeamAssistant.Gateway.Services.Render;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.RandomCoffee.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.WebUI.Contracts;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Prometheus.DotNetRuntime;

namespace Inc.TeamAssistant.Gateway.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTelemetry(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        
        DotNetRuntimeStatsBuilder
            .Customize()
            .WithGcStats()
            .WithThreadPoolStats()
            .StartCollecting();
        
        services
            .AddHealthChecks();
        
        return services;
    }
    
    public static IServiceCollection AddValidators(this IServiceCollection services, LanguageId languageId)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(languageId);

        ValidatorOptions.Global.LanguageManager.Culture = new(languageId.Value);
        ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Continue;
        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;

        services
            .AddValidatorsFromAssemblyContaining<IStoryRepository>(
                lifetime: ServiceLifetime.Scoped,
                includeInternalTypes: true)
            .AddValidatorsFromAssemblyContaining<ILocationsRepository>(
                lifetime: ServiceLifetime.Scoped,
                includeInternalTypes: true)
            .AddValidatorsFromAssemblyContaining<ITaskForReviewRepository>(
                lifetime: ServiceLifetime.Scoped,
                includeInternalTypes: true)
            .AddValidatorsFromAssemblyContaining<ITeamRepository>(
                lifetime: ServiceLifetime.Scoped,
                includeInternalTypes: true)
            .AddValidatorsFromAssemblyContaining<IRandomCoffeeRepository>(
                lifetime: ServiceLifetime.Scoped,
                includeInternalTypes: true);
        
        return services;
    }

    public static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddMediatR(c =>
            {
                c.Lifetime = ServiceLifetime.Scoped;
                c.RegisterServicesFromAssemblyContaining<IStoryRepository>();
                c.RegisterServicesFromAssemblyContaining<ILocationsRepository>();
                c.RegisterServicesFromAssemblyContaining<ITaskForReviewRepository>();
                c.RegisterServicesFromAssemblyContaining<ITeamRepository>();
                c.RegisterServicesFromAssemblyContaining<IRandomCoffeeRepository>();
            })
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>))
            .TryAddEnumerable(ServiceDescriptor.Scoped(
                typeof(IPipelineBehavior<,>),
                typeof(ValidationPipelineBehavior<,>)));

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