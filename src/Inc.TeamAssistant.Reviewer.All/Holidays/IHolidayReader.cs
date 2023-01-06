using Inc.TeamAssistant.Reviewer.All.Holidays.Model;

namespace Inc.TeamAssistant.Reviewer.All.Holidays;

public interface IHolidayReader
{
    Task<Dictionary<DateOnly, HolidayType>> GetAll(CancellationToken cancellationToken);
}