namespace Inc.TeamAssistant.Reviewer.Application;

public sealed class WorkdayOptions
{
    public bool WorkOnHoliday { get; set; }
    public TimeSpan StartTimeUtc { get; set; }
    public TimeSpan EndTimeUtc { get; set; }
    public TimeSpan NotificationInterval { get; set; }
}