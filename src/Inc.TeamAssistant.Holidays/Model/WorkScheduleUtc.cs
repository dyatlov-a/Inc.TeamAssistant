namespace Inc.TeamAssistant.Holidays.Model;

public sealed record WorkScheduleUtc(TimeOnly Start, TimeOnly End)
{
    public bool IsWorkTime(DateTimeOffset value)
    {
        if (value.TimeOfDay.Ticks < Start.Ticks)
            return false;
        
        if (value.TimeOfDay.Ticks >= End.Ticks)
            return false;
        
        return true;
    }
}