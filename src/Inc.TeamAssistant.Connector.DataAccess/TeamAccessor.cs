using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;
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

    public async Task<CurrentTeamContext> GetTeamContext(Guid teamId, CancellationToken token)
    {
        var team = await _teamRepository.Find(teamId, token);
        if (team is null)
            throw new TeamAssistantException($"Team by id {teamId} was not found.");

        return new(team.Id, team.Name, team.Properties, team.BotId);
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

    public async Task<Person> EnsurePerson(long personId, CancellationToken token)
    {
        var person = await _personRepository.Find(personId, token);
        if (person is null)
            throw new TeamAssistantUserException(Messages.Connector_PersonNotFound, personId);

        return person;
    }

    public async Task<LanguageId> GetClientLanguage(Guid botId, long personId, CancellationToken token)
    {
        return await _clientLanguageRepository.Get(botId, personId, token);
    }

    public async Task<bool> HasManagerAccess(Guid teamId, long personId, CancellationToken token)
    {
        return await _teamRepository.HasManagerAccess(teamId, personId, token);
    }

    public async Task EnsureManagerAccess(Guid teamId, long personId, CancellationToken token)
    {
        if (!await HasManagerAccess(teamId, personId, token))
            throw new ApplicationException($"User {personId} has not rights to remove teammate from team {teamId}");
    }
}