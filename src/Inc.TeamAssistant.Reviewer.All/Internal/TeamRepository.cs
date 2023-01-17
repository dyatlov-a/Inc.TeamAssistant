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
    t.name AS name,
    t.next_reviewer_type AS nextreviewertype
FROM review.teams AS t
WHERE t.id = @team_id;

SELECT
    p.id AS id,
    p.team_id AS teamid,
    p.person__id AS personid,
    p.person__language_id AS personlanguageid,
    p.person__first_name AS personfirstname,
    p.person__last_name AS personlastname,
    p.person__username AS personusername
FROM review.players AS p
WHERE p.team_id = @team_id;",
            new { team_id = teamId },
            flags: CommandFlags.None,
            cancellationToken: cancellationToken);

        await using var connection = new NpgsqlConnection(_connectionString);

        var query = await connection.QueryMultipleAsync(command);

        var team = await query.ReadSingleOrDefaultAsync<Team>();
        var players = await query.ReadAsync<DbPlayer>();

        return team?.Build(players.Select(p => Player.Build(
                p.Id,
                p.TeamId,
                new Person(p.PersonId, p.PersonLanguageId, p.PersonFirstName, p.PersonLastName, p.PersonUsername)))
            .ToArray());
    }

    public async Task<Player?> FindLastReviewer(Guid teamId, CancellationToken cancellationToken)
    {
        var command = new CommandDefinition(@"
SELECT
    p.id AS id,
    p.team_id AS teamid,
    p.person__id AS personid,
    p.person__language_id AS personlanguageid,
    p.person__first_name AS personfirstname,
    p.person__last_name AS personlastname,
    p.person__username AS personusername
FROM 
    review.task_for_reviews AS t
    review.players AS p ON p.id = t.reviewer_id
WHERE t.team_id = @team_id
ORDER BY t.created DESC
OFFSET 0
LIMIT 1;",
            new { team_id = teamId },
            flags: CommandFlags.None,
            cancellationToken: cancellationToken);

        await using var connection = new NpgsqlConnection(_connectionString);

        var result = await connection.QuerySingleOrDefaultAsync<DbPlayer>(command);
        return result is {}
            ? Player.Build(
                result.Id,
                result.TeamId,
                new Person(
                    result.PersonId,
                    result.PersonLanguageId,
                    result.PersonFirstName,
                    result.PersonLastName,
                    result.PersonUsername))
            : null;
    }

    public async Task<IReadOnlyCollection<(Guid Id, string Name)>> GetTeams(
        long userId,
        CancellationToken cancellationToken)
    {
        var command = new CommandDefinition(@"
SELECT DISTINCT ON (ot.id)
    ot.id AS id,
    ot.name AS name
FROM review.teams AS pt
JOIN review.players AS p ON p.team_id = pt.id
JOIN review.teams AS ot ON ot.chat_id = pt.chat_id
WHERE p.person__id = @person__id
ORDER BY ot.id, ot.name;",
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
        var personIds = new List<long>(team.Players.Count);
        var personLanguageIds = new List<string>(team.Players.Count);
        var personFirstNames = new List<string>(team.Players.Count);
        var personLastNames = new List<string?>(team.Players.Count);
        var personUsernames = new List<string?>(team.Players.Count);

        foreach (var player in team.Players)
        {
            playerIds.Add(player.Id);
            playerTeamIds.Add(player.TeamId);
            personIds.Add(player.Person.Id);
            personLanguageIds.Add(player.Person.LanguageId.Value);
            personFirstNames.Add(player.Person.FirstName);
            personLastNames.Add(player.Person.LastName);
            personUsernames.Add(player.Person.Username);
        }

        var command = new CommandDefinition(@"
INSERT INTO review.teams (id, chat_id, name, next_reviewer_type)
VALUES (@id, @chat_id, @name, @next_reviewer_type)
ON CONFLICT (id) DO UPDATE SET
chat_id = excluded.chat_id,
name = excluded.name,
next_reviewer_type = excluded.next_reviewer_type;

INSERT INTO review.players (
    id,
    team_id,
    person__id,
    person__language_id,
    person__first_name,
    person__last_name,
    person__username)
SELECT
    p.id,
    p.team_id,
    p.person__id,
    p.person__language_id,
    p.person__first_name,
    p.person__last_name,
    p.person__username
FROM UNNEST(
    @player_ids,
    @player_team_ids,
    @person__ids,
    @person__language_ids,
    @person__first_names,
    @person__last_names,
    @person__usernames)
AS p(
    id,
    team_id,
    person__id,
    person__language_id,
    person__first_name,
    person__last_name,
    person__username)
ON CONFLICT (id) DO UPDATE SET
team_id = excluded.team_id,
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
                next_reviewer_type = team.NextReviewerType,
                
                player_ids = playerIds,
                player_team_ids = playerTeamIds,
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