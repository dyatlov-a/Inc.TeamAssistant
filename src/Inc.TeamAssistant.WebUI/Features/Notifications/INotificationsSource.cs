namespace Inc.TeamAssistant.WebUI.Features.Notifications;

public interface INotificationsSource : IDisposable
{
    void OnChanged(Action action);
}