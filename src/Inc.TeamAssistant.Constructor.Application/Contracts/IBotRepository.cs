using Inc.TeamAssistant.Constructor.Domain;

namespace Inc.TeamAssistant.Constructor.Application.Contracts;

public interface IBotRepository
{
    Task<Bot?> Find(Guid id, CancellationToken token);
    
    Task Upsert(Bot bot, CancellationToken token);

    Task Remove(Guid id, CancellationToken token);
}