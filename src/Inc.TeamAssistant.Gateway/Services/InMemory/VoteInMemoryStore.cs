using System.Collections.Concurrent;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Gateway.Services.InMemory;

internal sealed class VoteInMemoryStore : IVoteStore
{
    private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<long, IReadOnlyCollection<VoteTicket>>> _state
        = new();

    public IReadOnlyCollection<VoteTicket> Get(Guid sessionId)
    {
        var result = _state.TryGetValue(sessionId, out var tickets)
            ? tickets.SelectMany(t => t.Value).ToArray()
            : [];

        return result;
    }

    public void Clear(Guid sessionId) => _state.TryRemove(sessionId, out _);

    public IReadOnlyCollection<VoteTicket> Get(Guid sessionId, long personId)
    {
        var result = _state.TryGetValue(sessionId, out var tickets) && tickets.TryGetValue(personId, out var byPerson)
            ? byPerson
            : [];
        
        return result;
    }
    
    public void Set(Guid sessionId, long personId, IReadOnlyCollection<VoteTicket> votes)
    {
        ArgumentNullException.ThrowIfNull(votes);
        
        var tickets = _state.GetOrAdd(
            sessionId,
            _ => new ConcurrentDictionary<long, IReadOnlyCollection<VoteTicket>>());
        
        tickets.AddOrUpdate(personId, k => votes, (k, v) => votes);
    }
}