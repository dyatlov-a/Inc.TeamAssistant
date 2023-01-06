namespace Inc.TeamAssistant.Reviewer.All.Holidays;

public interface IHolidayService
{
    Task<bool> IsWorkday(DateOnly date, CancellationToken cancellationToken);
}