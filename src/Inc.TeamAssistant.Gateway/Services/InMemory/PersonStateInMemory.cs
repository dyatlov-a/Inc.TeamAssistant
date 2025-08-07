using System.Collections.Concurrent;
using Inc.TeamAssistant.Primitives.Features.Tenants;

namespace Inc.TeamAssistant.Gateway.Services.InMemory;

internal sealed class PersonStateInMemory : IPersonState
{
    private readonly ConcurrentDictionary<RoomId, ConcurrentDictionary<long, PersonStateTicket>> _state = new();
    
    public IReadOnlyCollection<PersonStateTicket> Get(RoomId roomId)
    {
        ArgumentNullException.ThrowIfNull(roomId);
        
        var result = _state.TryGetValue(roomId, out var tickets)
            ? tickets.Select(t => t.Value).ToArray()
            : [];

        return result;
    }

    public void Set(RoomId roomId, PersonStateTicket ticket)
    {
        ArgumentNullException.ThrowIfNull(roomId);
        ArgumentNullException.ThrowIfNull(ticket);
        
        var tickets = _state.GetOrAdd(
            roomId,
            _ => new ConcurrentDictionary<long, PersonStateTicket>());
        
        tickets.AddOrUpdate(ticket.Person.Id, k => ticket, (k, v) => ticket);
    }

    public void Clear(RoomId roomId)
    {
        ArgumentNullException.ThrowIfNull(roomId);
        
        _state.TryRemove(roomId, out _);
    }
}