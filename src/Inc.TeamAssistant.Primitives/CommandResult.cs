namespace Inc.TeamAssistant.Primitives;

public sealed record CommandResult(IReadOnlyCollection<NotificationMessage> Notifications)
{
    public static readonly CommandResult Empty = new(Array.Empty<NotificationMessage>());
    
    public static CommandResult Build(params NotificationMessage[] notifications)
    {
        if (notifications is null)
            throw new ArgumentNullException(nameof(notifications));

        return new CommandResult(notifications);
    }
}