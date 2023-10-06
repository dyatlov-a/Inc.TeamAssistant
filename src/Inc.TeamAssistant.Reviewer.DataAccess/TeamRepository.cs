using Dapper;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;
using Npgsql;

namespace Inc.TeamAssistant.Reviewer.DataAccess;

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
    p.language_id AS languageid,
    p.first_name AS firstname,
    p.last_name AS lastname,
    p.username AS username
FROM review.persons AS p
JOIN review.players AS tp ON tp.team_id = @team_id AND tp.person_id = p.id;",
            new { team_id = teamId },
            flags: CommandFlags.None,
            cancellationToken: cancellationToken);

        await using var connection = new NpgsqlConnection(_connectionString);

        var query = await connection.QueryMultipleAsync(command);

        var team = await query.ReadSingleOrDefaultAsync<Team>();
        var players = await query.ReadAsync<Person>();

        return team?.Build(players.ToArray());
    }

    public async Task<IReadOnlyCollection<(Guid Id, string Name)>> GetTeamNames(
        long userId,
        long chatId,
        CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        
        var chatsCommand = new CommandDefinition(@"
            SELECT DISTINCT t.chat_id AS chatid
            FROM review.players AS p
            JOIN review.teams AS t ON t.id = p.team_id
            WHERE p.person_id = @person_id;",
            new { person_id = userId },
            flags: CommandFlags.None,
            cancellationToken: cancellationToken);
        var chats = await connection.QueryAsync<long>(chatsCommand);
        
        var chatIds = chats.Append(chatId).Distinct().ToArray();
        var teamsCommand = new CommandDefinition(@"
            SELECT t.id AS id, t.name AS name
            FROM review.teams AS t
            WHERE t.chat_Id = ANY(@chat_ids);",
            new { chat_ids = chatIds },
            flags: CommandFlags.None,
            cancellationToken: cancellationToken);
        var teams = await connection.QueryAsync<(Guid Id, string Name)>(teamsCommand);
        
        return teams.ToArray();
    }

    public async Task Upsert(Team team, CancellationToken cancellationToken)
    {
        if (team is null)
            throw new ArgumentNullException(nameof(team));
        
        var personIds = new List<long>(team.Players.Count);
        var personLanguageIds = new List<string>(team.Players.Count);
        var personFirstNames = new List<string>(team.Players.Count);
        var personLastNames = new List<string?>(team.Players.Count);
        var personUsernames = new List<string?>(team.Players.Count);

        foreach (var player in team.Players)
        {
            personIds.Add(player.Id);
            personLanguageIds.Add(player.LanguageId.Value);
            personFirstNames.Add(player.FirstName);
            personLastNames.Add(player.LastName);
            personUsernames.Add(player.Username);
        }

        var command = new CommandDefinition(@"
INSERT INTO review.teams (id, chat_id, name, next_reviewer_type)
VALUES (@id, @chat_id, @name, @next_reviewer_type)
ON CONFLICT (id) DO UPDATE SET
    chat_id = excluded.chat_id,
    name = excluded.name,
    next_reviewer_type = excluded.next_reviewer_type;

INSERT INTO review.persons (id, language_id, first_name, last_name, username)
SELECT p.id, p.language_id, p.first_name, p.last_name, p.username
FROM UNNEST(@person__ids, @person__language_ids, @person__first_names, @person__last_names, @person__usernames)
AS p(id, language_id, first_name, last_name, username)
ON CONFLICT (id) DO UPDATE SET
    language_id = excluded.language_id,
    first_name = excluded.first_name,
    last_name = excluded.last_name,
    username = excluded.username;

INSERT INTO review.players (team_id, person_id)
SELECT @id, person_id
FROM UNNEST(@person__ids)
AS p(person_id)
ON CONFLICT (team_id, person_id) DO NOTHING;",
            new
            {
                id = team.Id,
                chat_id = team.ChatId,
                name = team.Name,
                next_reviewer_type = team.NextReviewerType,
                
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