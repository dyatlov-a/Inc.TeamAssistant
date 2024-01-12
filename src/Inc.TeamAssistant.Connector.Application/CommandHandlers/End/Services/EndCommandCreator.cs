using Inc.TeamAssistant.Connector.Model.Commands.End;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.End.Services;

internal sealed class EndCommandCreator : ICommandCreator
{
    public string Command => "/cancel";
    
    public Task<IRequest<CommandResult>> Create(
        MessageContext messageContext,
        Guid? selectedTeamId,
        CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        
        return Task.FromResult<IRequest<CommandResult>>(new EndCommand(messageContext));
    }
}