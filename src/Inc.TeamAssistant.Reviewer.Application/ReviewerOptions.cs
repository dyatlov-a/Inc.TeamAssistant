namespace Inc.TeamAssistant.Reviewer.Application;

public sealed class ReviewerOptions
{
    public TimeSpan NotificationsDelay { get; set; }
    public TimeSpan NotificationInterval { get; set; }
    public string[] LinksPrefix { get; set; } = Array.Empty<string>();
}