using FluentValidation;
using Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AcceptEstimate.Services;
using Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AddStory.Services;
using Inc.TeamAssistant.Appraiser.Application.CommandHandlers.ReVoteEstimate.Services;
using Inc.TeamAssistant.Appraiser.Application.CommandHandlers.SetEstimateForStory.Services;
using Inc.TeamAssistant.Appraiser.Application.PipelineBehaviors;
using Inc.TeamAssistant.Appraiser.Application.Services;
using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Primitives;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Inc.TeamAssistant.Appraiser.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppraiserApplication(
        this IServiceCollection services,
        IConfiguration configuration)
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
            .AddSingleton<ICommandCreator, AcceptEstimateCommandCreator>()
            .AddSingleton<ICommandCreator, AddStoryCommandCreator>()
            .AddSingleton<ICommandCreator, ReVoteEstimateCommandCreator>();

        foreach (var assessment in AssessmentValue.GetAllAssessments())
        {
            var command = $"/set?value={assessment}&storyId=";
            
            services.AddSingleton<ICommandCreator>(
                sp => ActivatorUtilities.CreateInstance<SetEstimateForStoryCommandCreator>(sp, command));
        }

        services
            .AddScoped<SummaryByStoryBuilder>()
            .AddSingleton(addStoryOptions)
            .AddValidatorsFromAssemblyContaining<SetEstimateForStoryCommandCreator>(
                lifetime: ServiceLifetime.Scoped,
                includeInternalTypes: true)
            .TryAddEnumerable(ServiceDescriptor.Scoped(
                typeof(IPipelineBehavior<,>),
                typeof(ValidationPipelineBehavior<,>)));

        return services;
    }
}