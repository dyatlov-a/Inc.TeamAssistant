using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Retro.Application.Contracts;

public interface IRetroReader
{
    Task<IReadOnlyCollection<RetroItem>> GetAll(Guid teamId, CancellationToken token);
    
    Task<RetroSession?> FindActive(Guid teamId, CancellationToken token);
}