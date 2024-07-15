using Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AcceptEstimate.Services;
using Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AddStory.Services;
using Inc.TeamAssistant.Appraiser.Application.CommandHandlers.FinishEstimate.Services;
using Inc.TeamAssistant.Appraiser.Application.CommandHandlers.ReVoteEstimate.Services;
using Inc.TeamAssistant.Appraiser.Application.CommandHandlers.SetEstimateForStory.Services;
using Inc.TeamAssistant.Appraiser.Application.Services;
using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.FeatureProperties;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.Appraiser.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppraiserApplication(
        this IServiceCollection services,
        AppraiserOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);

        services
            .AddSingleton<ISettingSectionProvider, AppraiserSettingSectionProvider>()
            .AddScoped<SummaryByStoryBuilder>()
            .AddSingleton(options)
            
            .AddSingleton<ICommandCreator, AcceptEstimateCommandCreator>()
            .AddSingleton<ICommandCreator, AddStoryCommandCreator>()
            .AddSingleton<ICommandCreator, ReVoteEstimateCommandCreator>()
            .AddSingleton<ICommandCreator, FinishEstimateCommandCreator>();

        foreach (var assessment in AssessmentValue.GetAllAssessments())
        {
            var command = string.Format(CommandList.Set, assessment);
            
            services.AddSingleton<ICommandCreator>(
                sp => ActivatorUtilities.CreateInstance<SetEstimateForStoryCommandCreator>(sp, command, assessment));
        }
        
        return services;
    }
}