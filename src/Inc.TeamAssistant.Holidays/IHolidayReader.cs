using Inc.TeamAssistant.Holidays.Model;

namespace Inc.TeamAssistant.Holidays;

public interface IHolidayReader
{
    Task<Calendar?> Find(Guid botId, CancellationToken token);
}