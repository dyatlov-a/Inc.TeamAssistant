using System.Collections.Concurrent;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Features.Tenants;

namespace Inc.TeamAssistant.Gateway.Services.InMemory;

internal sealed class OnlinePersonInMemoryStore : IOnlinePersonStore
{
    private readonly ConcurrentDictionary<RoomId, ConcurrentDictionary<string, Person>> _state = new();

    public string? FindConnectionId(RoomId roomId, long personId)
    {
        ArgumentNullException.ThrowIfNull(roomId);
        
        if (_state.TryGetValue(roomId, out var persons))
            foreach (var person in persons)
                if (person.Value.Id == personId)
                    return person.Key;
        
        return null;
    }

    public IReadOnlyCollection<Person> GetPersons(RoomId roomId)
    {
        ArgumentNullException.ThrowIfNull(roomId);
        
        var result = _state.TryGetValue(roomId, out var persons)
            ? persons.Values.ToArray()
            : [];
        
        return result;
    }
    
    public IReadOnlyCollection<Person> JoinToRoom(RoomId roomId, string connectionId, Person person)
    {
        ArgumentNullException.ThrowIfNull(roomId);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionId);
        ArgumentNullException.ThrowIfNull(person);
        
        var persons = _state.GetOrAdd(roomId, _ => new ConcurrentDictionary<string, Person>());
        
        persons.TryAdd(connectionId, person);

        var newPersons = GetPersons(roomId);
        return newPersons;
    }

    public IReadOnlyCollection<Person> LeaveFromRoom(RoomId roomId, string connectionId)
    {
        ArgumentNullException.ThrowIfNull(roomId);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionId);
        
        if (_state.TryGetValue(roomId, out var persons))
            persons.TryRemove(connectionId, out _);
        
        var newPersons = GetPersons(roomId);
        return newPersons;
    }

    public IEnumerable<RoomId> LeaveFromRooms(string connectionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionId);

        return Leave();

        IEnumerable<RoomId> Leave()
        {
            foreach (var (roomId, personsLookup) in _state)
                if (personsLookup.TryRemove(connectionId, out _))
                    yield return roomId;
        }
    }
}