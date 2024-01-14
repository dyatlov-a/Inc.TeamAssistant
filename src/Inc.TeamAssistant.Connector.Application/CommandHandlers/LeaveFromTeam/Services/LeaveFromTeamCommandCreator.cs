using Inc.TeamAssistant.Connector.Model.Commands.LeaveFromTeam;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.LeaveFromTeam.Services;

internal sealed class LeaveFromTeamCommandCreator : ICommandCreator
{
    public string Command => "/leave_team";
    
    public Task<IRequest<CommandResult>> Create(
        MessageContext messageContext,
        CurrentTeamContext? teamContext,
        CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        
        if (!Guid.TryParse(messageContext.Text.TrimStart('/'), out var value))
            throw new ApplicationException("Can not bot leave from team. Please select target team.");
        
        return Task.FromResult<IRequest<CommandResult>>(new LeaveFromTeamCommand(messageContext, value));
    }
}