namespace Inc.TeamAssistant.Holidays;

public sealed class WorkdayOptions
{
    public bool WorkOnHoliday { get; set; }
    public TimeSpan Timezone { get; set; }
    public TimeSpan StartTimeUtc { get; set; }
    public TimeSpan EndTimeUtc { get; set; }
    public HashSet<DayOfWeek> Weekends { get; set; } = default!;
}