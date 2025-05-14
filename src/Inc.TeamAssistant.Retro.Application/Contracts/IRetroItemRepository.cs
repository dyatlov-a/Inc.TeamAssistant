using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Retro.Application.Contracts;

public interface IRetroItemRepository
{
    Task<RetroItem?> Find(Guid id, CancellationToken token);
    
    Task Upsert(RetroItem item, CancellationToken token);
    
    Task Remove(RetroItem item, CancellationToken token);
}