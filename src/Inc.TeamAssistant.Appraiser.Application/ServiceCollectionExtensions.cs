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
        string connectToDashboardLinkTemplate)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectToDashboardLinkTemplate);

        services
            .AddSingleton<ISettingSectionProvider, AppraiserSettingSectionProvider>()
            .AddScoped(sp => ActivatorUtilities.CreateInstance<SummaryByStoryBuilder>(sp, connectToDashboardLinkTemplate))
            .AddSingleton<ICommandCreator, AddStoryCommandCreator>()
            .AddSingleton<ICommandCreator, ReVoteEstimateCommandCreator>()
            .AddSingleton<ICommandCreator, FinishEstimateCommandCreator>()
            .AddSingleton<ICommandCreator, MoveToFibonacciCommandCreator>()
            .AddSingleton<ICommandCreator, MoveToTShirtsCommandCreator>()
            .AddSingleton<ICommandCreator, MoveToPowerOfTwoCommandCreator>();
        
        foreach (var assessment in EstimationStrategyFactory.GetAllValues())
        {
            var setCommand = string.Format(CommandList.Set, assessment);
            services.AddSingleton<ICommandCreator>(
                sp => ActivatorUtilities.CreateInstance<SetEstimateForStoryCommandCreator>(
                    sp,
                    setCommand,
                    assessment));

            var acceptCommand = string.Format(CommandList.AcceptEstimate, assessment);
            services.AddSingleton<ICommandCreator>(
                sp => ActivatorUtilities.CreateInstance<AcceptEstimateCommandCreator>(
                    sp,
                    acceptCommand,
                    assessment));
        }

        return services;
    }
}