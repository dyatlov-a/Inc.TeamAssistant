using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Retro.Application.Contracts;

public interface IRetroSessionReader
{
    Task<IReadOnlyCollection<RetroItem>> ReadRetroItems(
        Guid roomId,
        IReadOnlyCollection<RetroSessionState> states,
        CancellationToken token);
    
    Task<RetroSession?> FindSession(
        Guid roomId,
        IReadOnlyCollection<RetroSessionState> states,
        CancellationToken token);
    
    Task<IReadOnlyCollection<ActionItem>> ReadActionItems(
        Guid teamId,
        CancellationToken token);
}