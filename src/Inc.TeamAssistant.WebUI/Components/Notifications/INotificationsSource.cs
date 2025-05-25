namespace Inc.TeamAssistant.WebUI.Components.Notifications;

public interface INotificationsSource : IDisposable
{
    IReadOnlyCollection<Notification> Notifications { get; }
    
    void OnChanged(Action action);

    void Remove(Notification notification);
}