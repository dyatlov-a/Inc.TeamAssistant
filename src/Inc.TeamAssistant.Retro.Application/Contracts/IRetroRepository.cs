using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Retro.Application.Contracts;

public interface IRetroRepository
{
    Task<RetroItem?> FindItem(Guid id, CancellationToken token);
    
    Task Upsert(RetroItem item, CancellationToken token);
    
    Task Create(RetroSession retro, CancellationToken token);
    
    Task Remove(RetroItem item, CancellationToken token);
}