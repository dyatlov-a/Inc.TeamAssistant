namespace Inc.TeamAssistant.WebUI.Features.Notifications;

public sealed class Notification
{
    public DateTimeOffset Created { get; }
    public NotificationType Type { get; }
    public string Message { get; }
    
    private Notification(DateTimeOffset created, NotificationType type, string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        Created = created;
        Type = type;
        Message = message;
    }
    
    public static Notification Info(string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        
        return new Notification(DateTimeOffset.UtcNow, NotificationType.Info, message);
    }

    public static Notification Warning(string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        
        return new Notification(DateTimeOffset.UtcNow, NotificationType.Warning, message);
    }

    public static Notification Error(string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        
        return new Notification(DateTimeOffset.UtcNow, NotificationType.Error, message);
    }
}