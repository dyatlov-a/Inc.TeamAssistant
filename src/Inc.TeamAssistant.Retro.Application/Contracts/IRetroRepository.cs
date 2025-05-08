using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Retro.Application.Contracts;

public interface IRetroRepository
{
    Task<RetroItem?> FindItem(Guid id, CancellationToken token);
    
    Task Upsert(RetroItem item, CancellationToken token);
}