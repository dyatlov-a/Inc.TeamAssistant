using MediatR;

namespace Inc.TeamAssistant.Primitives;

public interface ICommandCreator
{
    string Command { get; }
    
    Task<IRequest<CommandResult>> Create(MessageContext messageContext, Guid? selectedTeamId, CancellationToken token);
}