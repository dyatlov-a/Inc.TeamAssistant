namespace Inc.TeamAssistant.WebUI.Features.Notifications;

public sealed class Notification
{
    public NotificationType Type { get; }
    public string Message { get; }
    
    private Notification(NotificationType type, string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        
        Type = type;
        Message = message;
    }
    
    public static Notification Info(string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        
        return new Notification(NotificationType.Info, message);
    }

    public static Notification Warning(string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        
        return new Notification(NotificationType.Warning, message);
    }

    public static Notification Error(string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        
        return new Notification(NotificationType.Error, message);
    }
}