namespace Inc.TeamAssistant.Holidays;

public interface IHolidayService
{
    Task<bool> IsWorkTime(Guid botId, DateTimeOffset value, CancellationToken token);

    Task<TimeSpan> CalculateWorkTime(Guid botId, DateTimeOffset start, DateTimeOffset end, CancellationToken token);
}