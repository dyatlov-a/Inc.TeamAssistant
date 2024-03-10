using Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AcceptEstimate.Services;
using Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AddStory.Services;
using Inc.TeamAssistant.Appraiser.Application.CommandHandlers.ReVoteEstimate.Services;
using Inc.TeamAssistant.Appraiser.Application.CommandHandlers.SetEstimateForStory.Services;
using Inc.TeamAssistant.Appraiser.Application.Services;
using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.Appraiser.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppraiserApplication(
        this IServiceCollection services,
        AppraiserOptions options)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));
        if (options is null)
            throw new ArgumentNullException(nameof(options));

        services
            .AddScoped<SummaryByStoryBuilder>()
            .AddSingleton(options)
            
            .AddSingleton<ICommandCreator, AcceptEstimateCommandCreator>()
            .AddSingleton<ICommandCreator, AddStoryCommandCreator>()
            .AddSingleton<ICommandCreator, ReVoteEstimateCommandCreator>();

        foreach (var assessment in AssessmentValue.GetAllAssessments())
        {
            var command = string.Format(CommandList.Set, assessment);
            
            services.AddSingleton<ICommandCreator>(
                sp => ActivatorUtilities.CreateInstance<SetEstimateForStoryCommandCreator>(sp, command, assessment));
        }
        
        return services;
    }
}