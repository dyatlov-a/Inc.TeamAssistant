namespace Inc.TeamAssistant.Holidays;

public interface IHolidayService
{
    Task<bool> IsWorkTime(DateTimeOffset value, CancellationToken token);

    Task<TimeSpan> CalculateWorkTime(DateTimeOffset start, DateTimeOffset end, CancellationToken token);
}