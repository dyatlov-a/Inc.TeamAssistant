using Dapper;
using Inc.TeamAssistant.Reviewer.All.Contracts;
using Inc.TeamAssistant.Reviewer.All.Model;
using Inc.TeamAssistant.Reviewer.All.Services;
using Npgsql;

namespace Inc.TeamAssistant.Reviewer.All.Internal;

internal sealed class PersonsReader : IPersonsReader
{
    private readonly string _connectionString;

    public PersonsReader(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));

        _connectionString = connectionString;
    }
    
    public async Task<Person?> FindLastReviewer(Guid teamId, CancellationToken cancellationToken)
    {
        var command = new CommandDefinition(@"
SELECT
    p.id AS id,
    p.language_id AS languageid,
    p.first_name AS firstname,
    p.last_name AS lastname,
    p.username AS username
FROM review.task_for_reviews AS t
JOIN review.persons AS p ON p.id = t.reviewer_id
WHERE t.team_id = @team_id
ORDER BY t.created DESC
OFFSET 0
LIMIT 1;",
            new { team_id = teamId },
            flags: CommandFlags.None,
            cancellationToken: cancellationToken);

        await using var connection = new NpgsqlConnection(_connectionString);

        return await connection.QuerySingleOrDefaultAsync<Person>(command);
    }

    public async Task<Person?> Find(UserIdentity userIdentity, CancellationToken cancellationToken)
    {
        if (userIdentity is null)
            throw new ArgumentNullException(nameof(userIdentity));
        
        var command = new CommandDefinition(@"
SELECT
    p.id AS id,
    p.language_id AS languageid,
    p.first_name AS firstname,
    p.last_name AS lastname,
    p.username AS username
FROM review.persons AS p
WHERE (p.person__id = @person_id AND @username is null) OR (p.person__username = @username AND @person_id is null)
ORDER BY p.id
OFFSET 0
LIMIT 1;",
            new { person_id = userIdentity.UserId, username = userIdentity.Username },
            flags: CommandFlags.None,
            cancellationToken: cancellationToken);

        await using var connection = new NpgsqlConnection(_connectionString);

        return await connection.QuerySingleOrDefaultAsync<Person>(command);
    }
}