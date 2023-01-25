using Inc.TeamAssistant.Reviewer.All.Model;
using Inc.TeamAssistant.Reviewer.All.Services;

namespace Inc.TeamAssistant.Reviewer.All.Contracts;

public interface IPersonsReader
{
    Task<Person?> FindLastReviewer(Guid teamId, CancellationToken cancellationToken = default);

    Task<Person?> Find(UserIdentity userIdentity, CancellationToken cancellationToken = default);
}