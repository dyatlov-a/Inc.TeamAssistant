using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Connector.DataAccess;

internal sealed class PersonAccessor : IPersonAccessor
{
    private readonly IPersonRepository _repository;

    public PersonAccessor(IPersonRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<Person> Ensure(Person person, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(person);
        
        await _repository.Upsert(person, token);

        return person;
    }
}