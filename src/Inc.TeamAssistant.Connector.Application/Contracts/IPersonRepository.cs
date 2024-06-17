using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Connector.Application.Contracts;

public interface IPersonRepository
{
    Task<Person?> Find(long personId, CancellationToken token);
    Task<Person?> Find(string username, CancellationToken token);
    Task<IReadOnlyCollection<Person>> GetTeammates(Guid teamId, DateTimeOffset now, CancellationToken token);
    Task Upsert(Person person, CancellationToken token);
    Task LeaveFromTeam(Guid teamId, long personId, CancellationToken token);
    Task LeaveFromTeam(Guid teamId, long personId, DateTimeOffset? until, CancellationToken token);
}