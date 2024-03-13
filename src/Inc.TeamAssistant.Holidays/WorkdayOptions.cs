namespace Inc.TeamAssistant.Holidays;

public sealed class WorkdayOptions
{
    public bool WorkOnHoliday { get; set; }
    public TimeSpan StartTimeUtc { get; set; }
    public TimeSpan EndTimeUtc { get; set; }
}