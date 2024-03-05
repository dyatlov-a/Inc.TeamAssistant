using Inc.TeamAssistant.Connector.Model.Commands.CreateTeam;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.CreateTeam.Services;

internal sealed class CreateTeamCommandCreator : ICommandCreator
{
    public string Command => "/new_team";
    
    public Task<IRequest<CommandResult>> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        if (teamContext is null)
            throw new ArgumentNullException(nameof(teamContext));
            
        return Task.FromResult<IRequest<CommandResult>>(new CreateTeamCommand(
            messageContext,
            messageContext.BotName,
            messageContext.Text,
            teamContext.Properties));
    }
}