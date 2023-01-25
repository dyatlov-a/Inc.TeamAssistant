using Inc.TeamAssistant.Reviewer.All.Model;
using Inc.TeamAssistant.Reviewer.All.Services;

namespace Inc.TeamAssistant.Reviewer.All.Contracts;

public interface IPersonRepository
{
    Task<Person?> FindLastReviewer(Guid teamId, CancellationToken cancellationToken);

    Task<Person?> Find(UserIdentity userIdentity, CancellationToken cancellationToken);

    Task Upsert(Person person, CancellationToken cancellationToken);
}