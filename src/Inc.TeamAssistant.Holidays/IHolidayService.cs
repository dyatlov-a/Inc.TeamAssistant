namespace Inc.TeamAssistant.Holidays;

public interface IHolidayService
{
    Task<bool> IsWorkTime(DateTimeOffset value, CancellationToken token);
    
    DateTimeOffset GetLastDayOfWeek(DayOfWeek dayOfWeek, DateTimeOffset date);
}