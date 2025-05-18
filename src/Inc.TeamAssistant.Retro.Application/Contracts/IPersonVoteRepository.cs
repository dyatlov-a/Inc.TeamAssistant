using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Retro.Application.Contracts;

public interface IPersonVoteRepository
{
    Task Upsert(PersonVote personVote, CancellationToken token);
}