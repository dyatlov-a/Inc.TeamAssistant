using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Connector.Application.Contracts;

public interface IPersonRepository
{
    Task<Person?> Find(long personId, CancellationToken token);
    Task<Person?> Find(string username, CancellationToken token);
    Task<Teammate?> Find(TeammateKey key, CancellationToken token);
    Task<IReadOnlyCollection<Person>> GetTeammates(
        Guid teamId,
        DateTimeOffset now,
        bool? canFinalize,
        CancellationToken token);
    Task Upsert(Person person, CancellationToken token);
    Task RemoveFromTeam(TeammateKey key, CancellationToken token);
    Task Upsert(Teammate teammate, CancellationToken token);
    Task<Guid?> FindBotId(long personId, CancellationToken token);
}