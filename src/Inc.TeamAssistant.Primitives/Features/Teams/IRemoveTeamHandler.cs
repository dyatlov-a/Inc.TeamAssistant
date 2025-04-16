using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Primitives.Features.Teams;

public interface IRemoveTeamHandler
{
    Task Handle(MessageContext messageContext, Guid teamId, CancellationToken token);
}