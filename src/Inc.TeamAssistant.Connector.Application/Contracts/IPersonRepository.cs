using Inc.TeamAssistant.Connector.Domain;

namespace Inc.TeamAssistant.Connector.Application.Contracts;

public interface IPersonRepository
{
    Task<Person?> Find(long personId, CancellationToken token);

    Task Upsert(Person person, CancellationToken token);
}