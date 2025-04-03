using System.Text;

namespace Inc.TeamAssistant.Primitives.Notifications;

public sealed class NotificationBuilder
{
    private readonly StringBuilder _builder = new();
    
    private NotificationBuilder()
    {
    }
    
    public static NotificationBuilder Create() => new();
    
    public NotificationBuilder AddIfHasValue(string? value, Action<StringBuilder> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        if (!string.IsNullOrWhiteSpace(value))
            action(_builder);
        
        return this;
    }

    public NotificationBuilder AddItems<T>(IEnumerable<T> items, Action<StringBuilder, T> action)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(action);

        foreach (var item in items)
            action(_builder, item);
        
        return this;
    }
    
    public NotificationBuilder Add(Action<StringBuilder> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        
        action(_builder);

        return this;
    }

    public NotificationBuilder AddIf(bool condition, Action<StringBuilder> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        
        if (condition)
            action(_builder);

        return this;
    }

    public NotificationMessage Build(Func<string, NotificationMessage> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        var notification = action(_builder.ToString());
        
        return notification;
    }
}