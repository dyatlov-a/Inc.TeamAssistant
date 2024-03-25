using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Primitives.Handlers;

public interface ILeaveTeamHandler
{
    Task Handle(MessageContext messageContext, Guid teamId, CancellationToken token);
}