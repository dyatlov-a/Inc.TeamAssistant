using Dapper;
using Inc.TeamAssistant.Reviewer.All.Contracts;
using Inc.TeamAssistant.Reviewer.All.Model;
using Inc.TeamAssistant.Reviewer.All.Services;
using Npgsql;

namespace Inc.TeamAssistant.Reviewer.All.Internal;

internal sealed class PlayersReader : IPlayersReader
{
    private readonly string _connectionString;

    public PlayersReader(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));

        _connectionString = connectionString;
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
FROM review.task_for_reviews AS t
JOIN review.players AS p ON p.id = t.reviewer_id
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

    public async Task<Player?> Find(UserIdentity userIdentity, Guid? teamId, CancellationToken cancellationToken)
    {
        if (userIdentity is null)
            throw new ArgumentNullException(nameof(userIdentity));
        
        var command = new CommandDefinition(@"
SELECT
    p.id AS id,
    p.team_id AS teamid,
    p.person__id AS personid,
    p.person__language_id AS personlanguageid,
    p.person__first_name AS personfirstname,
    p.person__last_name AS personlastname,
    p.person__username AS personusername
FROM review.players AS p
WHERE (@team_id IS NULL OR p.team_id = @team_id)
    AND ((p.person__id = @person_id AND @username is null) OR (p.person__username = @username AND @person_id is null))
ORDER BY p.person__id
OFFSET 0
LIMIT 1;",
            new { team_id = teamId, person_id = userIdentity.UserId, username = userIdentity.Username },
            flags: CommandFlags.None,
            cancellationToken: cancellationToken);

        await using var connection = new NpgsqlConnection(_connectionString);

        var result = await connection.QueryFirstOrDefaultAsync<DbPlayer>(command);
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
}