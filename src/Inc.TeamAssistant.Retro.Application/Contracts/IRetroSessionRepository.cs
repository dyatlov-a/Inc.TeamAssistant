using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Retro.Application.Contracts;

public interface IRetroSessionRepository
{
    Task<RetroSession?> Find(Guid id, CancellationToken token);
    
    Task Create(RetroSession retro, CancellationToken token);

    Task Update(RetroSession retro, CancellationToken token);
}