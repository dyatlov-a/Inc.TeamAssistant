using FluentValidation;
using Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AddStoryToAssessmentSession;
using Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AddStoryToAssessmentSession.Validators;
using Inc.TeamAssistant.Appraiser.Application.PipelineBehaviors;
using Inc.TeamAssistant.Appraiser.Application.Services;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Inc.TeamAssistant.Appraiser.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));
        if (configuration is null)
            throw new ArgumentNullException(nameof(configuration));

        var addStoryOptions = configuration
            .GetRequiredSection(nameof(AddStoryToAssessmentSessionOptions))
            .Get<AddStoryToAssessmentSessionOptions>()!;

        ValidatorOptions.Global.LanguageManager.Culture = new("en");
        ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Continue;
        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;

        services
            .AddScoped<SummaryByStoryBuilder>()
            .AddSingleton(addStoryOptions)
            .AddValidatorsFromAssemblyContaining<AddStoryToAssessmentSessionCommandValidator>(
                lifetime: ServiceLifetime.Scoped,
                includeInternalTypes: true)
            .TryAddEnumerable(ServiceDescriptor.Scoped(
                typeof(IPipelineBehavior<,>),
                typeof(ValidationPipelineBehavior<,>)));

        return services;
    }
}