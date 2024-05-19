using Inc.TeamAssistant.Connector.Domain;

namespace Inc.TeamAssistant.Connector.Application.Contracts;

public interface IPhotosRepository
{
    Task<IReadOnlyCollection<(long PersonId, Guid BotId)>> Get(DateTimeOffset fromDate, CancellationToken token);
    
    Task<Photo?> Find(long personId, CancellationToken token);
    
    Task Upsert(Photo photo, CancellationToken token);
}