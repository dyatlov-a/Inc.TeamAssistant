using System.Collections.Concurrent;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Gateway.Services.Stores;

internal sealed class VoteStore : IVoteStore
{
    private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<long, IReadOnlyCollection<VoteTicket>>> _state = new();
    
    public IReadOnlyCollection<VoteTicket> GetVotes(Guid teamId, long personId)
    {
        var result = _state.TryGetValue(teamId, out var tickets) && tickets.TryGetValue(personId, out var byPerson)
            ? byPerson
            : [];
        
        return result;
    }
    
    public void SetVotes(Guid teamId, long personId, IReadOnlyCollection<VoteTicket> votes)
    {
        var tickets = _state.GetOrAdd(teamId, _ => new ConcurrentDictionary<long, IReadOnlyCollection<VoteTicket>>());
        
        tickets.AddOrUpdate(personId, k => votes, (k, v) => votes);
    }
}