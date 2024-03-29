using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Handlers;
using Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToAccept.Services;
using Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToDecline.Services;
using Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToInProgress.Services;
using Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToNextRound.Services;
using Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToReview.Services;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.Reviewer.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddReviewerApplication(this IServiceCollection services, ReviewerOptions options)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));
        if (options is null)
            throw new ArgumentNullException(nameof(options));

        services
            .AddSingleton(options)
            .AddScoped<IMessageBuilderService, MessageBuilderService>()
            .AddHostedService<NotificationsService>()
            
            .AddSingleton<ICommandCreator, MoveToAcceptCommandCreator>()
            .AddSingleton<ICommandCreator, MoveToDeclineCommandCreator>()
            .AddSingleton<ICommandCreator, MoveToInProgressCommandCreator>()
            .AddSingleton<ICommandCreator, MoveToNextRoundCommandCreator>()
            .AddSingleton<ICommandCreator, MoveToReviewCommandCreator>()

            .AddSingleton<ILeaveTeamHandler, LeaveTeamHandler>();

        return services;
    }
}