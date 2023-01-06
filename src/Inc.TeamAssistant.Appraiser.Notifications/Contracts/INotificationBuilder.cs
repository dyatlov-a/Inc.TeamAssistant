namespace Inc.TeamAssistant.Appraiser.Notifications.Contracts;

public interface INotificationBuilder<in TCommandResult>
{
	IAsyncEnumerable<NotificationMessage> Build(TCommandResult commandResult, long fromId);
}