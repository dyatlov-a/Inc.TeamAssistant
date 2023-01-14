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
    p.user_id AS userid,
    p.team_id AS teamid,
    p.name AS name,
    p.last_reviewer_id AS lastreviewerid,
    p.language_id AS languageid,
    p.login AS login
FROM review.players AS p
WHERE p.team_id = @team_id;",
            new { team_id = teamId },
            flags: CommandFlags.None,
            cancellationToken: cancellationToken);

        await using var connection = new NpgsqlConnection(_connectionString);

        var query = await connection.QueryMultipleAsync(command);

        var team = await query.ReadSingleOrDefaultAsync<Team>();
        var players = await query.ReadAsync<Player>();
        return team?.Build(players.ToArray());
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
WHERE p.user_id = @user_id
ORDER BY t.name;",
            new { user_id = userId },
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
        var playerUserIds = new List<long>(team.Players.Count);
        var playerTeamIds = new List<Guid>(team.Players.Count);
        var playerNames = new List<string>(team.Players.Count);
        var playerLastReviewerIds = new List<long?>(team.Players.Count);
        var playerLanguageIds = new List<string>(team.Players.Count);
        var playerLogins = new List<string?>(team.Players.Count);

        foreach (var player in team.Players)
        {
            playerIds.Add(player.Id);
            playerUserIds.Add(player.UserId);
            playerTeamIds.Add(player.TeamId);
            playerNames.Add(player.Name);
            playerLastReviewerIds.Add(player.LastReviewerId);
            playerLanguageIds.Add(player.LanguageId.Value);
            playerLogins.Add(player.Login);
        }

        var command = new CommandDefinition(@"
INSERT INTO review.teams (id, chat_id, name)
VALUES (@id, @chat_id, @name)
ON CONFLICT (id) DO UPDATE SET
chat_id = excluded.chat_id,
name = excluded.name;

INSERT INTO review.players (id, user_id, team_id, name, last_reviewer_id, language_id, login)
SELECT p.id, p.user_id, p.team_id, p.name, p.last_reviewer_id, p.player_language_id, p.player_login
FROM UNNEST(@player_ids, @player_user_ids, @player_team_ids, @player_names, @player_last_reviewer_ids, @player_language_ids, @player_logins)
AS p(id, user_id, team_id, name, last_reviewer_id, player_language_id, player_login)
ON CONFLICT (id) DO UPDATE SET
user_id = excluded.user_id,
team_id = excluded.team_id,
name = excluded.name,
last_reviewer_id = excluded.last_reviewer_id,
language_id = excluded.language_id,
login = excluded.login;",
            new
            {
                id = team.Id,
                chat_id = team.ChatId,
                name = team.Name,
                player_ids = playerIds,
                player_user_ids = playerUserIds,
                player_team_ids = playerTeamIds,
                player_names = playerNames,
                player_last_reviewer_ids = playerLastReviewerIds,
                player_language_ids = playerLanguageIds,
                player_logins = playerLogins
            },
            flags: CommandFlags.None,
            cancellationToken: cancellationToken);

        await using var connection = new NpgsqlConnection(_connectionString);

        await connection.ExecuteAsync(command);
    }
}