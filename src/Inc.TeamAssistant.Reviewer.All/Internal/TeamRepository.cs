using Dapper;
using Inc.TeamAssistant.Reviewer.All.Contracts;
using Inc.TeamAssistant.Reviewer.All.Model;
using Npgsql;

namespace Inc.TeamAssistant.Reviewer.All.Internal;

internal sealed class TeamRepository : ITeamRepository
{
    private readonly string _connectionString;

    public TeamRepository(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));

        _connectionString = connectionString;
    }

    public async Task<Team?> Find(Guid teamId, CancellationToken cancellationToken)
    {
        var command = new CommandDefinition(@"
SELECT
    t.id AS id,
    t.chat_id AS chatid,
    t.name AS name
FROM review.teams AS t
WHERE t.id = @team_id;

SELECT
    p.id AS id,
    p.team_id AS teamid,
    p.last_reviewer_id AS lastreviewerid
FROM review.players AS p
WHERE p.team_id = @team_id;

SELECT
    p.id AS playerid,
    p.person__id AS id,
    p.person__language_id AS languageid,
    p.person__first_name AS firstname,
    p.person__last_name AS lastname,
    p.person__username AS username
FROM review.players AS p
WHERE p.team_id = @team_id;",
            new { team_id = teamId },
            flags: CommandFlags.None,
            cancellationToken: cancellationToken);

        await using var connection = new NpgsqlConnection(_connectionString);

        var query = await connection.QueryMultipleAsync(command);

        var team = await query.ReadSingleOrDefaultAsync<Team>();
        var players = await query.ReadAsync<Player>();
        var personsLookup = (await query.ReadAsync<DbPerson>()).ToDictionary(
            p => p.PlayerId,
            p => new Person(p.Id, p.LanguageId, p.FirstName, p.LastName, p.Username));
        return team?.Build(players.Select(p => p.Build(personsLookup[p.Id])).ToArray());
    }

    public async Task<IReadOnlyCollection<(Guid Id, string Name)>> GetTeams(
        long userId,
        CancellationToken cancellationToken)
    {
        var command = new CommandDefinition(@"
SELECT
    t.id AS id,
    t.name AS name
FROM review.teams AS t
JOIN review.players AS p ON p.team_id = t.id
WHERE p.person__id = @person__id
ORDER BY t.name;",
            new { person__id = userId },
            flags: CommandFlags.None,
            cancellationToken: cancellationToken);

        await using var connection = new NpgsqlConnection(_connectionString);

        var results = await connection.QueryAsync<(Guid Id, string Name)>(command);
        return results.ToArray();
    }

    public async Task Upsert(Team team, CancellationToken cancellationToken)
    {
        if (team is null)
            throw new ArgumentNullException(nameof(team));

        var playerIds = new List<Guid>(team.Players.Count);
        var playerTeamIds = new List<Guid>(team.Players.Count);
        var playerLastReviewerIds = new List<long?>(team.Players.Count);
        var personIds = new List<long>(team.Players.Count);
        var personLanguageIds = new List<string>(team.Players.Count);
        var personFirstNames = new List<string>(team.Players.Count);
        var personLastNames = new List<string?>(team.Players.Count);
        var personUsernames = new List<string?>(team.Players.Count);

        foreach (var player in team.Players)
        {
            playerIds.Add(player.Id);
            playerTeamIds.Add(player.TeamId);
            playerLastReviewerIds.Add(player.LastReviewerId);
            personIds.Add(player.Person.Id);
            personLanguageIds.Add(player.Person.LanguageId.Value);
            personFirstNames.Add(player.Person.FirstName);
            personLastNames.Add(player.Person.LastName);
            personUsernames.Add(player.Person.Username);
        }

        var command = new CommandDefinition(@"
INSERT INTO review.teams (id, chat_id, name)
VALUES (@id, @chat_id, @name)
ON CONFLICT (id) DO UPDATE SET
chat_id = excluded.chat_id,
name = excluded.name;

INSERT INTO review.players (
    id, team_id,
    last_reviewer_id,
    person__id,
    person__language_id,
    person__first_name,
    person__last_name,
    person__username)
SELECT
    p.id,
    p.team_id,
    p.last_reviewer_id,
    p.person__id,
    p.person__language_id,
    p.person__first_name,
    p.person__last_name,
    p.person__username
FROM UNNEST(
    @player_ids,
    @player_team_ids,
    @player_last_reviewer_ids,
    @person__ids,
    @person__language_ids,
    @person__first_names,
    @person__last_names,
    @person__usernames)
AS p(
    id,
    team_id,
    last_reviewer_id,
    person__id,
    person__language_id,
    person__first_name,
    person__last_name,
    person__username)
ON CONFLICT (id) DO UPDATE SET
team_id = excluded.team_id,
last_reviewer_id = excluded.last_reviewer_id,
person__id = excluded.person__id,
person__language_id = excluded.person__language_id,
person__first_name = excluded.person__first_name,
person__last_name = excluded.person__last_name,
person__username = excluded.person__username;",
            new
            {
                id = team.Id,
                chat_id = team.ChatId,
                name = team.Name,
                
                player_ids = playerIds,
                player_team_ids = playerTeamIds,
                player_last_reviewer_ids = playerLastReviewerIds,
                
                person__ids = personIds,
                person__language_ids = personLanguageIds,
                person__first_names = personFirstNames,
                person__last_names = personLastNames,
                person__usernames = personUsernames
            },
            flags: CommandFlags.None,
            cancellationToken: cancellationToken);

        await using var connection = new NpgsqlConnection(_connectionString);

        await connection.ExecuteAsync(command);
    }
}