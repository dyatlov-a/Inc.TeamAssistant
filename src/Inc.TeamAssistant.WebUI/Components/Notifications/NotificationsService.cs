using System.Timers;

namespace Inc.TeamAssistant.WebUI.Components.Notifications;

internal sealed class NotificationsService : INotificationsSource, INotificationsService
{
    private readonly ILogger<NotificationsService> _logger;
    private readonly List<Notification> _notifications = new();
    private readonly System.Timers.Timer _timer;
    
    private Action? _changed;

    public NotificationsService(ILogger<NotificationsService> logger, TimeSpan messageLifetime)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _timer = CreateTimer(messageLifetime);
    }
    
    public IReadOnlyCollection<Notification> Notifications => _notifications;
    
    public void Publish(Notification notification)
    {
        ArgumentNullException.ThrowIfNull(notification);
        
        _notifications.Add(notification);

        _changed?.Invoke();
        
        _timer.Stop();
        _timer.Start();
    }
    
    public void Remove(Notification notification)
    {
        ArgumentNullException.ThrowIfNull(notification);
        
        _notifications.Remove(notification);
    }
    
    public void OnChanged(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);
        
        if (_changed is not null)
            throw new ApplicationException("Only one listener is supported");
        
        _changed = action;
    }
    
    private void TryCheckMessages()
    {
        try
        {
            var notifications = _notifications
                .Where(n => n.Type != NotificationType.Error)
                .ToArray();

            foreach (var notification in notifications)
                Remove(notification);
        
            _changed?.Invoke();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on timer tick for NotificationsService");
        }
    }
    
    private void HandleTick(object? sender, ElapsedEventArgs e)
    {
        _timer.Stop();
        
        TryCheckMessages();
    }

    private System.Timers.Timer CreateTimer(TimeSpan interval)
    {
        var timer = new System.Timers.Timer(interval);
        
        timer.Elapsed += HandleTick;
        timer.AutoReset = false;

        return timer;
    }

    public void Dispose()
    {
        _timer.Stop();
        _timer.Elapsed -= HandleTick;
        _timer.Dispose();
    }
}