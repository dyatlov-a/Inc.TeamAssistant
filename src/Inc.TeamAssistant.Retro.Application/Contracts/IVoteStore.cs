using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Retro.Application.Contracts;

public interface IVoteStore
{
    IReadOnlyCollection<VoteTicket> GetVotes(Guid teamId, long personId);
    
    void SetVotes(Guid teamId, long personId, IReadOnlyCollection<VoteTicket> votes);
}