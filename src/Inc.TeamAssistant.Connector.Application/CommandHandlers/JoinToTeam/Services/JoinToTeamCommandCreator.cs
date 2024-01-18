using Inc.TeamAssistant.Connector.Model.Commands.JoinToTeam;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.JoinToTeam.Services;

internal sealed class JoinToTeamCommandCreator : ICommandCreator
{
    public string Command => "/start";
    
    public Task<IRequest<CommandResult>> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        if (teamContext is null)
            throw new ArgumentNullException(nameof(teamContext));
        
        return Task.FromResult<IRequest<CommandResult>>(new JoinToTeamCommand(
            messageContext,
            messageContext.TryParseId(Command)));
    }
}