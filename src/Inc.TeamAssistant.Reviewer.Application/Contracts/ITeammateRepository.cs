using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Contracts;

public interface ITeammateRepository
{
    Task<Teammate?> Find(TeammateKey key, CancellationToken token);
    Task RemoveFromTeam(TeammateKey key, CancellationToken token);
    Task Upsert(Teammate teammate, CancellationToken token);
}