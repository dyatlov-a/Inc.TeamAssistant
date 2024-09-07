namespace Inc.TeamAssistant.WebUI.Features.Notifications;

public sealed class NotificationsService
{
    private readonly List<Notification> _notifications = new();
    private Action? _changed;
    
    public IReadOnlyCollection<Notification> Notifications => _notifications;
    
    public void OnChanged(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);
        
        _changed = action;
    }
    
    public void Add(Notification notification)
    {
        ArgumentNullException.ThrowIfNull(notification);
        
        _notifications.Add(notification);

        if (_changed is not null)
            _changed();
    }
    
    public void Remove(Notification notification)
    {
        ArgumentNullException.ThrowIfNull(notification);
        
        _notifications.Remove(notification);
    }
}