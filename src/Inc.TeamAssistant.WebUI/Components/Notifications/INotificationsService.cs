namespace Inc.TeamAssistant.WebUI.Components.Notifications;

public interface INotificationsService
{
    void Publish(Notification notification);
}