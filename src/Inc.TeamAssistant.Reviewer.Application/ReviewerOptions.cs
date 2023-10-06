namespace Inc.TeamAssistant.Reviewer.Application;

public sealed class ReviewerOptions
{
    public string BotLink { get; set; } = default!;
    public string BotName { get; set; } = default!;
    public string LinkForConnectTemplate { get; set; } = default!;
    public string AccessToken { get; set; } = default!;
    public TimeSpan NotificationsDelay { get; set; }
    public int NotificationsBatch { get; set; }
    public WorkdayOptions Workday { get; set; } = default!;
}