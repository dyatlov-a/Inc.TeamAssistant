using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Features.Properties;
using Inc.TeamAssistant.Primitives.Features.Teams;
using Inc.TeamAssistant.Reviewer.Application.CommandHandlers.AddComment.Services;
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
            .AddHostedService<PushBackgroundService>()
            
            .AddSingleton<ISettingSectionProvider, ReviewerSettingSectionProvider>()
            .AddSingleton<ReviewMetricsProvider>()
            .AddSingleton<IReviewMetricsProvider>(sp => sp.GetRequiredService<ReviewMetricsProvider>())
            .AddSingleton<IReviewMetricsLoader>(sp => sp.GetRequiredService<ReviewMetricsProvider>())
            .AddSingleton<ReviewTeamMetricsFactory>()
            .AddSingleton<ReviewCommentsProvider>()
            .AddHostedService<WarmCacheHostedService>()
            
            .AddSingleton<ICommandCreator, MoveToAcceptCommandCreator>()
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
            .AddSingleton<ICommandCreator, ChangeToRoundRobinForTeamCommandCreator>()
            .AddSingleton<ICommandCreator, AddCommentCommandCreator>()

            .AddScoped<ILeaveTeamHandler, LeaveTeamHandler>()
            .AddScoped<IRemoveTeamHandler, RemoveTeamHandler>();

        return services;
    }
}