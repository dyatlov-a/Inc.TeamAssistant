using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Retro.Application.Contracts;

public interface IOnlinePersonStore
{
    string? FindConnectionId(Guid teamId, long personId);
    
    IReadOnlyCollection<Person> GetPersons(Guid teamId);
    
    IReadOnlyCollection<Person> JoinToTeam(Guid teamId, string connectionId, Person person);

    IReadOnlyCollection<Person> LeaveFromTeam(Guid teamId, string connectionId);

    IAsyncEnumerable<Guid> LeaveFromTeams(
        string connectionId,
        Func<Guid, IReadOnlyCollection<Person>, CancellationToken, Task> notify,
        CancellationToken token);
}