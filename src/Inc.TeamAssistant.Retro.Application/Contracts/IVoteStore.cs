using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Retro.Application.Contracts;

public interface IVoteStore
{
    IReadOnlyCollection<VoteTicket> Get(Guid sessionId);
    
    IReadOnlyCollection<VoteTicket> Get(Guid sessionId, long personId);
    
    void Set(Guid sessionId, long personId, IReadOnlyCollection<VoteTicket> votes);
    
    void Clear(Guid sessionId);
}