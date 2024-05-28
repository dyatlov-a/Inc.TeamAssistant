using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Handlers;
using Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToAccept.Services;
using Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToDecline.Services;
using Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToInProgress.Services;
using Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToNextRound.Services;
using Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToReview.Services;
using Inc.TeamAssistant.Reviewer.Application.CommandHandlers.ReassignReview.Services;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Application.Handlers;
using Inc.TeamAssistant.Reviewer.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.Reviewer.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddReviewerApplication(this IServiceCollection services, ReviewerOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);

        services
            .AddSingleton(options)
            .AddScoped<IReviewMessageBuilder, ReviewMessageBuilder>()
            .AddScoped<ReassignReviewService>()
            .AddSingleton<ReviewHistoryService>()
            .AddHostedService<PushService>()
            
            .AddSingleton<ICommandCreator, MoveToAcceptCommandCreator>()
            .AddSingleton<ICommandCreator, MoveToDeclineCommandCreator>()
            .AddSingleton<ICommandCreator, MoveToInProgressCommandCreator>()
            .AddSingleton<ICommandCreator, MoveToNextRoundCommandCreator>()
            .AddSingleton<ICommandCreator, MoveToReviewCommandCreator>()
            .AddSingleton<ICommandCreator, ReassignReviewCommandCreator>()

            .AddScoped<ILeaveTeamHandler, LeaveTeamHandler>()
            .AddScoped<IRemoveTeamHandler, RemoveTeamHandler>();

        return services;
    }
}