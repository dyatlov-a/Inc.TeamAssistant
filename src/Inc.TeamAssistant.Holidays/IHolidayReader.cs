using Inc.TeamAssistant.Holidays.Model;

namespace Inc.TeamAssistant.Holidays;

public interface IHolidayReader
{
    Task<Dictionary<DateOnly, HolidayType>> GetAll(CancellationToken cancellationToken);
}