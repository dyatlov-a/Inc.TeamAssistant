namespace Inc.TeamAssistant.Primitives;

public interface INotificationMessageSender
{
    Task Send(Guid teamId, NotificationMessage notification, CancellationToken token);
}