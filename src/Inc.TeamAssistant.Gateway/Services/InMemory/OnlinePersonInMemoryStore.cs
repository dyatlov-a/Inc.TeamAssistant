using System.Collections.Concurrent;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Features.Tenants;

namespace Inc.TeamAssistant.Gateway.Services.InMemory;

internal sealed class OnlinePersonInMemoryStore : IOnlinePersonStore
{
    private readonly ConcurrentDictionary<RoomId, ConcurrentDictionary<long, PersonStateTicketWrapper>> _state = new();

    public IReadOnlyCollection<string> GetConnections(RoomId roomId, long personId)
    {
        ArgumentNullException.ThrowIfNull(roomId);
        
        return _state.TryGetValue(roomId, out var persons) && persons.TryGetValue(personId, out var ticket)
            ? ticket.ConnectionIds
            : [];
    }

    public IReadOnlyCollection<PersonStateTicket> GetTickets(RoomId roomId)
    {
        ArgumentNullException.ThrowIfNull(roomId);
        
        var result = _state.TryGetValue(roomId, out var persons)
            ? persons.Values.Select(v => v.ToPersonStateTicket()).ToArray()
            : [];
        
        return result;
    }
    
    public void JoinToRoom(RoomId roomId, string connectionId, Person person)
    {
        ArgumentNullException.ThrowIfNull(roomId);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionId);
        ArgumentNullException.ThrowIfNull(person);
        
        var persons = _state.GetOrAdd(roomId, _ => new ConcurrentDictionary<long, PersonStateTicketWrapper>());
        var now = DateTimeOffset.UtcNow;
        
        persons.AddOrUpdate(
            person.Id,
            pId => new PersonStateTicketWrapper(person).Connected(connectionId, now),
            (pId, p) => p.Connected(connectionId, now));
    }

    public IEnumerable<RoomId> LeaveFromRooms(string connectionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionId);

        return Leave();

        IEnumerable<RoomId> Leave()
        {
            foreach (var tickets in _state.ToArray())
            foreach (var ticket in tickets.Value.ToArray())
                if (ticket.Value.Disconnected(connectionId))
                    yield return tickets.Key;
        }
    }

    public void SetTicket(RoomId roomId, Person person, int totalVote, bool finished, bool handRaised)
    {
        ArgumentNullException.ThrowIfNull(roomId);

        if (_state.TryGetValue(roomId, out var persons))
            persons.AddOrUpdate(
                person.Id,
                pId => new PersonStateTicketWrapper(person)
                    .ChangeTotalVote(totalVote)
                    .ChangeFinished(finished)
                    .ChangeHandRaised(handRaised),
                (pId, p) => p
                    .ChangeTotalVote(totalVote)
                    .ChangeFinished(finished)
                    .ChangeHandRaised(handRaised));
    }

    public void SetTicket(RoomId roomId, Person person, bool finished)
    {
        ArgumentNullException.ThrowIfNull(roomId);

        if (_state.TryGetValue(roomId, out var persons))
            persons.AddOrUpdate(
                person.Id,
                pId => new PersonStateTicketWrapper(person).ChangeFinished(finished),
                (pId, p) => p.ChangeFinished(finished));
    }

    public void ClearTickets(RoomId roomId)
    {
        ArgumentNullException.ThrowIfNull(roomId);
        
        if (_state.TryGetValue(roomId, out var persons))
            foreach (var person in persons.ToArray())
                person.Value.Clear();
    }

    public void Clear(DateTimeOffset now, TimeSpan idleConnectionLifetime)
    {
        var canDisconnect = now.Subtract(idleConnectionLifetime);
        var canDisconnectLookup = new Dictionary<RoomId, List<long>>();
        
        foreach (var tickets in _state.ToArray())
        foreach (var ticket in tickets.Value.ToArray())
            if (ticket.Value.LastConnection < canDisconnect && !ticket.Value.ConnectionIds.Any())
            {
                var roomId = tickets.Key;
                
                if (!canDisconnectLookup.ContainsKey(roomId))
                    canDisconnectLookup.Add(roomId, new List<long>());
                
                canDisconnectLookup[roomId].Add(ticket.Key);
            }

        foreach (var disconnect in canDisconnectLookup)
        foreach (var item in disconnect.Value)
            _state[disconnect.Key].Remove(item, out _);

        foreach (var item in _state.ToArray())
            if (!item.Value.Any())
                _state.Remove(item.Key, out _);
    }
}