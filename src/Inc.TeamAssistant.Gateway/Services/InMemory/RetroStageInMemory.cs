using System.Collections.Concurrent;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Gateway.Services.InMemory;

internal sealed class RetroStageInMemory : IRetroStage
{
    private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<long, RetroStageTicket>> _state = new();
    
    public IReadOnlyCollection<RetroStageTicket> Get(Guid roomId)
    {
        var result = _state.TryGetValue(roomId, out var tickets)
            ? tickets.Select(t => t.Value).ToArray()
            : [];

        return result;
    }

    public void Set(Guid roomId, RetroStageTicket ticket)
    {
        ArgumentNullException.ThrowIfNull(ticket);
        
        var tickets = _state.GetOrAdd(
            roomId,
            _ => new ConcurrentDictionary<long, RetroStageTicket>());
        
        tickets.AddOrUpdate(ticket.PersonId, k => ticket, (k, v) => ticket);
    }

    public void Clear(Guid roomId) => _state.TryRemove(roomId, out _);
}