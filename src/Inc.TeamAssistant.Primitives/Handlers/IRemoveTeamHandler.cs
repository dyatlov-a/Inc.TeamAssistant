using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Primitives.Handlers;

public interface IRemoveTeamHandler
{
    Task Handle(MessageContext messageContext, Guid teamId, CancellationToken token);
}