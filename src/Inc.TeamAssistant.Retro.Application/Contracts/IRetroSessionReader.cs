using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Retro.Application.Contracts;

public interface IRetroSessionReader
{
    Task<IReadOnlyCollection<RetroItem>> ReadAvailableItems(Guid roomId, CancellationToken token);

    Task<IReadOnlyCollection<RetroItem>> ReadItems(Guid retroSessionId, CancellationToken token);
    
    Task<RetroSession?> FindSession(
        Guid roomId,
        IReadOnlyCollection<RetroSessionState> states,
        CancellationToken token);
    
    Task<IReadOnlyCollection<ActionItem>> ReadActionItems(Guid retroSessionId, CancellationToken token);
}