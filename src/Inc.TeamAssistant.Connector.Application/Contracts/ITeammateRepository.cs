using Inc.TeamAssistant.Connector.Domain;

namespace Inc.TeamAssistant.Connector.Application.Contracts;

public interface ITeammateRepository
{
    Task<Teammate?> Find(TeammateKey key, CancellationToken token);
    Task RemoveFromTeam(TeammateKey key, CancellationToken token);
    Task Upsert(Teammate teammate, CancellationToken token);
}