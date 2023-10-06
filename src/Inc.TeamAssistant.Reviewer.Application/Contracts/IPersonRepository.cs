using Inc.TeamAssistant.Reviewer.Application.Services;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Users;

namespace Inc.TeamAssistant.Reviewer.Application.Contracts;

public interface IPersonRepository
{
    Task<Person?> FindLastReviewer(Guid teamId, CancellationToken cancellationToken);

    Task<Person?> Find(UserIdentity userIdentity, CancellationToken cancellationToken);

    Task Upsert(Person person, CancellationToken cancellationToken);
}