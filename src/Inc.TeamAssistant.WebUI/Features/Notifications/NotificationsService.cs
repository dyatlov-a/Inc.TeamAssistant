using System.Timers;

namespace Inc.TeamAssistant.WebUI.Features.Notifications;

public sealed class NotificationsService : INotificationsSource
{
    private readonly List<Notification> _notifications = new();
    private readonly System.Timers.Timer _timer;
    private readonly TimeSpan _messageLifetime;
    
    private Action? _changed;

    public NotificationsService(TimeSpan messageLifetime, TimeSpan checkInterval)
    {
        _messageLifetime = messageLifetime;
        _timer = new System.Timers.Timer(checkInterval);
        _timer.Elapsed += CheckMessages;
        _timer.Enabled = true;
    }
    
    public IReadOnlyCollection<Notification> Notifications => _notifications;
    
    public void Add(Notification notification)
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
    
    private void CheckMessages(object? sender, ElapsedEventArgs e)
    {
        var from = DateTimeOffset.UtcNow.Subtract(_messageLifetime);
        var notifications = _notifications
            .Where(n => n.Type != NotificationType.Error && n.Created < from)
            .ToArray();

        foreach (var notification in notifications)
            Remove(notification);
        
        _changed?.Invoke();
    }
    
    void INotificationsSource.OnChanged(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);
        
        _changed = action;
    }

    public void Dispose()
    {
        _timer.Elapsed -= CheckMessages;
        _timer.Dispose();
    }
}