namespace Inc.TeamAssistant.Holidays;

public interface IHolidayService
{
    Task<bool> IsWorkday(DateOnly date, CancellationToken token);
}