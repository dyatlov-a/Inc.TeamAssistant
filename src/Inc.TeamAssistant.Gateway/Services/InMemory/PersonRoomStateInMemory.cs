using System.Collections.Concurrent;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Tenants.Application.Contracts;
using Inc.TeamAssistant.Tenants.Domain;

namespace Inc.TeamAssistant.Gateway.Services.InMemory;

internal sealed class PersonRoomStateInMemory : IPersonRoomState
{
    private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<long, PersonRoomTicket>> _state = new();
    
    public IReadOnlyCollection<PersonRoomTicket> Get(Guid roomId)
    {
        var result = _state.TryGetValue(roomId, out var tickets)
            ? tickets.Select(t => t.Value).ToArray()
            : [];

        return result;
    }

    public void Set(Guid roomId, PersonRoomTicket ticket)
    {
        ArgumentNullException.ThrowIfNull(ticket);
        
        var tickets = _state.GetOrAdd(
            roomId,
            _ => new ConcurrentDictionary<long, PersonRoomTicket>());
        
        tickets.AddOrUpdate(ticket.PersonId, k => ticket, (k, v) => ticket);
    }

    public void Clear(Guid roomId) => _state.TryRemove(roomId, out _);
}