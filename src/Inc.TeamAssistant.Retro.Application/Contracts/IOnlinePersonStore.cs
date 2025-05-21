using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Retro.Application.Contracts;

public interface IOnlinePersonStore
{
    IReadOnlyCollection<Person> GetPersons(Guid teamId);
    
    IReadOnlyCollection<Person> JoinToTeam(string connectionId, Guid teamId, Person person);

    IReadOnlyCollection<Person> LeaveFromTeam(string connectionId, Guid teamId);

    IAsyncEnumerable<Guid> LeaveFromAllTeams(
        string connectionId,
        Func<Guid, IReadOnlyCollection<Person>, CancellationToken, Task> notify,
        CancellationToken token);
}