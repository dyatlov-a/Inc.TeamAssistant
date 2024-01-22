using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToReview;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToReview.Services;

internal sealed class MoveToReviewCommandCreator : ICommandCreator
{
    public string Command => CommandList.NeedReview;
    
    public Task<IRequest<CommandResult>> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        if (teamContext is null)
            throw new ArgumentNullException(nameof(teamContext));
            
        return Task.FromResult<IRequest<CommandResult>>(new MoveToReviewCommand(
            messageContext,
            teamContext.TeamId,
            teamContext.Properties.GetValueOrDefault("Strategy", NextReviewerType.RoundRobin.ToString()),
            messageContext.Text));
    }
}