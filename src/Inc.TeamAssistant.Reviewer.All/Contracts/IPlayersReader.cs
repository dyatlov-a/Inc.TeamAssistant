using Inc.TeamAssistant.Reviewer.All.Model;
using Inc.TeamAssistant.Reviewer.All.Services;

namespace Inc.TeamAssistant.Reviewer.All.Contracts;

public interface IPlayersReader
{
    Task<Player?> FindLastReviewer(Guid teamId, CancellationToken cancellationToken = default);

    Task<Player?> Find(UserIdentity userIdentity, Guid? teamId = null, CancellationToken cancellationToken = default);
}