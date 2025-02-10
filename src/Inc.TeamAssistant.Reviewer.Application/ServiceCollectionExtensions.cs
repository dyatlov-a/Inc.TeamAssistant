using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Handlers;
using Inc.TeamAssistant.Primitives.FeatureProperties;
using Inc.TeamAssistant.Reviewer.Application.CommandHandlers.CancelDraft.Services;
using Inc.TeamAssistant.Reviewer.Application.CommandHandlers.EditDraft.Services;
using Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToAccept.Services;
using Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToDecline.Services;
using Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToInProgress.Services;
using Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToNextRound.Services;
using Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToReview.Services;
using Inc.TeamAssistant.Reviewer.Application.CommandHandlers.NeedReview.Services;
using Inc.TeamAssistant.Reviewer.Application.CommandHandlers.ReassignReview.Services;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Application.Handlers;
using Inc.TeamAssistant.Reviewer.Application.QueryHandlers.GetLastTasks.Converters;
using Inc.TeamAssistant.Reviewer.Application.Services;
using Inc.TeamAssistant.Reviewer.Domain.NextReviewerStrategies;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.Reviewer.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddReviewerApplication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddSingleton<INextReviewerStrategyFactory, NextReviewerStrategyFactory>()
            .AddSingleton<DraftTaskForReviewService>()
            .AddScoped<IReviewMessageBuilder, ReviewMessageBuilder>()
            .AddScoped<ReassignReviewService>()
            .AddHostedService<PushService>()
            
            .AddSingleton<ISettingSectionProvider, ReviewerSettingSectionProvider>()
            .AddSingleton<ReviewMetricsProvider>()
            .AddSingleton<IReviewMetricsProvider>(sp => sp.GetRequiredService<ReviewMetricsProvider>())
            .AddSingleton<IReviewMetricsLoader>(sp => sp.GetRequiredService<ReviewMetricsProvider>())
            .AddSingleton<ReviewTeamMetricsFactory>()
            .AddSingleton<TaskForReviewHistoryConverter>()
            .AddHostedService<ReviewMetricsService>()
            
            .AddSingleton<ICommandCreator, MoveToAcceptCommandCreator>()
            .AddSingleton<ICommandCreator, MoveToAcceptWithCommentsCommandCreator>()
            .AddSingleton<ICommandCreator, MoveToDeclineCommandCreator>()
            .AddSingleton<ICommandCreator, MoveToInProgressCommandCreator>()
            .AddSingleton<ICommandCreator, MoveToNextRoundCommandCreator>()
            .AddSingleton<ICommandCreator, MoveToReviewCommandCreator>()
            .AddSingleton<ICommandCreator, NeedReviewCommandCreator>()
            .AddSingleton<ICommandCreator, ReassignReviewCommandCreator>()
            .AddSingleton<ICommandCreator, EditDraftCommandCreator>()
            .AddSingleton<ICommandCreator, CancelDraftCommandCreator>()
            .AddSingleton<ICommandCreator, ChangeToRandomCommandCreator>()
            .AddSingleton<ICommandCreator, ChangeToRoundRobinCommandCreator>()

            .AddScoped<ILeaveTeamHandler, LeaveTeamHandler>()
            .AddScoped<IRemoveTeamHandler, RemoveTeamHandler>();

        return services;
    }
}