using Inc.TeamAssistant.Connector.Model.Commands.LeaveFromTeam;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.LeaveFromTeam.Services;

internal sealed class LeaveFromTeamCommandCreator : ICommandCreator
{
    private readonly BotCommandStage _commandStage = BotCommandStage.SelectTeam;
    
    public int Priority => 3;
    
    public Task<IRequest<CommandResult>?> Create(MessageContext messageContext)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        
        if (messageContext.CurrentCommandStage == _commandStage)
            return Task.FromResult<IRequest<CommandResult>?>(new LeaveFromTeamCommand(
                messageContext,
                Guid.Parse(messageContext.Text.TrimStart('/'))));

        return Task.FromResult<IRequest<CommandResult>?>(null);
    }
}