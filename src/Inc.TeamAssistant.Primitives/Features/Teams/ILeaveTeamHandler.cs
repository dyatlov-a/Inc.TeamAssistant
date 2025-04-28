using Inc.TeamAssistant.Primitives.Notifications;

namespace Inc.TeamAssistant.Primitives.Features.Teams;

public interface ILeaveTeamHandler
{
    Task<IEnumerable<NotificationMessage>> Handle(TeammateKey key, CancellationToken token);
}