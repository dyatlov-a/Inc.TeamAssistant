using Inc.TeamAssistant.Holidays.Model;

namespace Inc.TeamAssistant.Constructor.Application.Contracts;

public interface ICalendarRepository
{
    Task<Calendar?> FindByOwner(long ownerId, CancellationToken token);
}