namespace Inc.TeamAssistant.WebUI.Features.Notifications;

internal sealed class NotificationsService : INotificationsSource, INotificationsService
{
    private readonly List<Notification> _notifications = new();
    
    private readonly TimeSpan _messageLifetime;
    private readonly TimeSpan _checkInterval;
    
    private Action? _changed;

    public NotificationsService(TimeSpan messageLifetime, TimeSpan checkInterval)
    {
        _messageLifetime = messageLifetime;
        _checkInterval = checkInterval;
    }
    
    public IReadOnlyCollection<Notification> Notifications => _notifications;
    
    public void Publish(Notification notification)
    {
        ArgumentNullException.ThrowIfNull(notification);
        
        _notifications.Add(notification);

        _changed?.Invoke();
    }
    
    public void Remove(Notification notification)
    {
        ArgumentNullException.ThrowIfNull(notification);
        
        _notifications.Remove(notification);
    }
    
    public IDisposable OnChanged(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);
        
        if (_changed is not null)
            throw new ApplicationException("Only one listener is supported");
        
        _changed = action;

        return new TimerScope(CheckMessages, _checkInterval);
    }
    
    private void CheckMessages()
    {
        var from = DateTimeOffset.UtcNow.Subtract(_messageLifetime);
        var notifications = _notifications
            .Where(n => n.Type != NotificationType.Error && n.Created < from)
            .ToArray();

        foreach (var notification in notifications)
            Remove(notification);
        
        _changed?.Invoke();
    }
}