namespace Inc.TeamAssistant.WebUI.Features.Notifications;

public interface INotificationsService
{
    void Publish(Notification notification);
}