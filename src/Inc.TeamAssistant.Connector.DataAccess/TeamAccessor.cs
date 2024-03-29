using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Connector.DataAccess;

internal sealed class TeamAccessor : ITeamAccessor
{
    private readonly IPersonRepository _personRepository;
    private readonly IClientLanguageRepository _clientLanguageRepository;

    public TeamAccessor(IPersonRepository personRepository, IClientLanguageRepository clientLanguageRepository)
    {
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
        _clientLanguageRepository = clientLanguageRepository ?? throw new ArgumentNullException(nameof(clientLanguageRepository));
    }

    public async Task<IReadOnlyCollection<Person>> GetTeammates(Guid teamId, CancellationToken token)
    {
        return await _personRepository.GetTeammates(teamId, token);
    }

    public async Task<Person?> FindPerson(long personId, CancellationToken token)
    {
        return await _personRepository.Find(personId, token);
    }

    public async Task<LanguageId> GetClientLanguage(long personId, CancellationToken token)
    {
        return await _clientLanguageRepository.Get(personId, token);
    }
}