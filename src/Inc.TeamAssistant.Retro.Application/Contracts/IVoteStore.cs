using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Retro.Application.Contracts;

public interface IVoteStore
{
    IReadOnlyCollection<VoteTicket> Get(Guid teamId);
    
    IReadOnlyCollection<VoteTicket> Get(Guid teamId, long personId);
    
    void Set(Guid teamId, long personId, IReadOnlyCollection<VoteTicket> votes);
    
    void Clear(Guid teamId);
}