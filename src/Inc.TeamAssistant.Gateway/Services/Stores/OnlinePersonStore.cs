using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Application.Contracts;

namespace Inc.TeamAssistant.Gateway.Services.Stores;

internal sealed class OnlinePersonStore : IOnlinePersonStore
{
    private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<string, Person>> _state = new();

    public string? FindConnectionId(Guid teamId, long personId)
    {
        if (_state.TryGetValue(teamId, out var persons))
            foreach (var person in persons)
                if (person.Value.Id == personId)
                    return person.Key;
        
        return null;
    }

    public IReadOnlyCollection<Person> GetPersons(Guid teamId)
    {
        var result = _state.TryGetValue(teamId, out var persons)
            ? persons.Values.ToArray()
            : [];
        
        return result;
    }
    
    public IReadOnlyCollection<Person> JoinToTeam(string connectionId, Guid teamId, Person person)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionId);
        ArgumentNullException.ThrowIfNull(person);
        
        var persons = _state.GetOrAdd(teamId, _ => new ConcurrentDictionary<string, Person>());
        
        persons.TryAdd(connectionId, person);

        return GetPersons(teamId);
    }

    public IReadOnlyCollection<Person> LeaveFromTeam(string connectionId, Guid teamId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionId);
        
        if (_state.TryGetValue(teamId, out var persons))
            persons.TryRemove(connectionId, out _);
        
        return GetPersons(teamId);
    }

    public async IAsyncEnumerable<Guid> LeaveFromAllTeams(
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