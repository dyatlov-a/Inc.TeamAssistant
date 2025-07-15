using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Tenants.Application.Contracts;

public interface IOnlinePersonStore
{
    string? FindConnectionId(Guid roomId, long personId);
    
    IReadOnlyCollection<Person> GetPersons(Guid roomId);
    
    IReadOnlyCollection<Person> JoinToTeam(Guid roomId, string connectionId, Person person);

    IReadOnlyCollection<Person> LeaveFromTeam(Guid roomId, string connectionId);

    IAsyncEnumerable<Guid> LeaveFromTeams(
        string connectionId,
        Func<Guid, IReadOnlyCollection<Person>, CancellationToken, Task> notify,
        CancellationToken token);
}