using Inc.TeamAssistant.Reviewer.All.Model;

namespace Inc.TeamAssistant.Reviewer.All.Contracts;

public interface ITeamRepository
{
    Task<Team?> Find(Guid teamId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<(Guid Id, string Name)>> GetTeamNames(
        long userId,
        long chatId,
        CancellationToken cancellationToken);

    Task Upsert(Team team, CancellationToken cancellationToken);
}