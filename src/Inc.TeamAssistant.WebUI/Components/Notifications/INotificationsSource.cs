namespace Inc.TeamAssistant.WebUI.Components.Notifications;

public interface INotificationsSource
{
    IReadOnlyCollection<Notification> Notifications { get; }
    
    IDisposable OnChanged(Action action);

    void Remove(Notification notification);
}