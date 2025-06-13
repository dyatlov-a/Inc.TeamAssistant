using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Application.Contracts;

namespace Inc.TeamAssistant.Gateway.Services.InMemory;

internal sealed class OnlinePersonInMemoryStore : IOnlinePersonStore
{
    private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<string, Person>> _state = new();

    public string? FindConnectionId(Guid roomId, long personId)
    {
        if (_state.TryGetValue(roomId, out var persons))
            foreach (var person in persons)
                if (person.Value.Id == personId)
                    return person.Key;
        
        return null;
    }

    public IReadOnlyCollection<Person> GetPersons(Guid roomId)
    {
        var result = _state.TryGetValue(roomId, out var persons)
            ? persons.Values.ToArray()
            : [];
        
        return result;
    }
    
    public IReadOnlyCollection<Person> JoinToTeam(Guid roomId, string connectionId, Person person)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionId);
        ArgumentNullException.ThrowIfNull(person);
        
        var persons = _state.GetOrAdd(roomId, _ => new ConcurrentDictionary<string, Person>());
        
        persons.TryAdd(connectionId, person);

        return GetPersons(roomId);
    }

    public IReadOnlyCollection<Person> LeaveFromTeam(Guid roomId, string connectionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionId);
        
        if (_state.TryGetValue(roomId, out var persons))
            persons.TryRemove(connectionId, out _);
        
        return GetPersons(roomId);
    }

    public async IAsyncEnumerable<Guid> LeaveFromTeams(
        string connectionId,
        Func<Guid, IReadOnlyCollection<Person>, CancellationToken, Task> notify,
        [EnumeratorCancellation] CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionId);
        ArgumentNullException.ThrowIfNull(notify);
        
        foreach (var (teamId, persons) in _state)
            if (persons.TryRemove(connectionId, out _))
            {
                await notify(teamId, GetPersons(teamId), token);

                yield return teamId;
            }
    }
}