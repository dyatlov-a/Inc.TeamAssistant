using Inc.TeamAssistant.Holidays.Model;

namespace Inc.TeamAssistant.Constructor.Application.Contracts;

public interface ICalendarRepository
{
    Task<IReadOnlyCollection<Guid>> GetBotIds(Guid calendarId, CancellationToken token);
    
    Task<Calendar> GetById(Guid id, CancellationToken token);
    
    Task<Calendar?> FindByOwner(long ownerId, CancellationToken token);

    Task Upsert(Calendar calendar, CancellationToken token);
}