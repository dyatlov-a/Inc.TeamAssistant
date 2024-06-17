using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Connector.DataAccess;

internal sealed class TeamAccessor : ITeamAccessor
{
    private readonly IPersonRepository _personRepository;
    private readonly IClientLanguageRepository _clientLanguageRepository;
    private readonly ITeamRepository _teamRepository;

    public TeamAccessor(
        IPersonRepository personRepository,
        IClientLanguageRepository clientLanguageRepository,
        ITeamRepository teamRepository)
    {
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
        _clientLanguageRepository =
            clientLanguageRepository ?? throw new ArgumentNullException(nameof(clientLanguageRepository));
        _teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
    }

    public async Task<(Guid BotId, string TeamName)> GetTeamContext(Guid teamId, CancellationToken token)
    {
        var team = await _teamRepository.Find(teamId, token);
        if (team is null)
            throw new TeamAssistantException($"Team by id {teamId} was not found.");

        return (team.BotId, team.Name);
    }

    public async Task<IReadOnlyCollection<Person>> GetTeammates(
        Guid teamId,
        DateTimeOffset now,
        CancellationToken token)
    {
        return await _personRepository.GetTeammates(teamId, now, token);
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