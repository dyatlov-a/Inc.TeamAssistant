using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Notifications;

namespace Inc.TeamAssistant.Primitives.Handlers;

public interface ILeaveTeamHandler
{
    Task<IEnumerable<NotificationMessage>> Handle(MessageContext messageContext, Guid teamId, CancellationToken token);
}