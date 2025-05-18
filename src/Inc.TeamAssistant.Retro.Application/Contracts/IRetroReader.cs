using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Retro.Application.Contracts;

public interface IRetroReader
{
    Task<IReadOnlyCollection<RetroItem>> ReadItems(
        Guid teamId,
        CancellationToken token);
    
    Task<RetroSession?> FindSession(
        Guid teamId,
        IReadOnlyCollection<RetroSessionState> states,
        CancellationToken token);
    
    Task<IReadOnlyCollection<PersonVote>> ReadVotes(
        Guid teamId,
        IReadOnlyCollection<RetroSessionState> states,
        CancellationToken token);
}