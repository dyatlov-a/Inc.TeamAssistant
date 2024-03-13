namespace Inc.TeamAssistant.Reviewer.Application;

public sealed class ReviewerOptions
{
    public TimeSpan NotificationsDelay { get; set; }
    public int NotificationsBatch { get; set; }
    public TimeSpan NotificationInterval { get; set; }
}