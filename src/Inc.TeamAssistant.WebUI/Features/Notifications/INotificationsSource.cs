namespace Inc.TeamAssistant.WebUI.Features.Notifications;

public interface INotificationsSource
{
    IReadOnlyCollection<Notification> Notifications { get; }
    
    IDisposable OnChanged(Action action);

    void Remove(Notification notification);
}