using Dapper;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Application.Services;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Users;
using Npgsql;

namespace Inc.TeamAssistant.Reviewer.DataAccess;

internal sealed class PersonRepository : IPersonRepository
{
    private readonly string _connectionString;

    public PersonRepository(string connectionString)
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
WHERE (p.id = @person_id AND @username is null) OR (p.username = @username AND @person_id is null)
ORDER BY p.id
OFFSET 0
LIMIT 1;",
            new { person_id = userIdentity.UserId, username = userIdentity.Username },
            flags: CommandFlags.None,
            cancellationToken: cancellationToken);

        await using var connection = new NpgsqlConnection(_connectionString);

        return await connection.QuerySingleOrDefaultAsync<Person>(command);
    }

    public async Task Upsert(Person person, CancellationToken cancellationToken)
    {
        if (person is null)
            throw new ArgumentNullException(nameof(person));

        var command = new CommandDefinition(@"
INSERT INTO review.persons (
    id,
    language_id,
    first_name,
    last_name,
    username)
VALUES (
    @id,
    @language_id,
    @first_name,
    @last_name,
    @username)
ON CONFLICT (id) DO UPDATE SET
    language_id = excluded.language_id,
    first_name = excluded.first_name,
    last_name = excluded.last_name,
    username = excluded.username;",
            new
            {
                id = person.Id,
                language_id = person.LanguageId,
                first_name = person.FirstName,
                last_name = person.LastName,
                username = person.Username
            },
            flags: CommandFlags.None,
            cancellationToken: cancellationToken);

        await using var connection = new NpgsqlConnection(_connectionString);

        await connection.ExecuteAsync(command);
    }
}