using Dapper;
using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Domain;
using Npgsql;

namespace Inc.TeamAssistant.Connector.DataAccess;

internal sealed class PersonRepository : IPersonRepository
{
    private readonly string _connectionString;

    public PersonRepository(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));
        
        _connectionString = connectionString;
    }
    
    public async Task<Person?> Find(long personId, CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT
                p.id AS id,
                p.name AS name,
                p.language_id AS languageid,
                p.username AS username
            FROM connector.persons AS p
            WHERE p.id = @person_id;",
            new { person_id = personId },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = new NpgsqlConnection(_connectionString);

        return await connection.QuerySingleOrDefaultAsync<Person>(command);
    }

    public async Task<Person?> Find(string username, CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT
                p.id AS id,
                p.name AS name,
                p.language_id AS languageid,
                p.username AS username
            FROM connector.persons AS p
            WHERE LOWER(p.username) = @username;",
            new { username = username.ToLowerInvariant() },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = new NpgsqlConnection(_connectionString);

        return await connection.QuerySingleOrDefaultAsync<Person>(command);
    }

    public async Task Upsert(Person person, CancellationToken token)
    {
        if (person is null)
            throw new ArgumentNullException(nameof(person));

        var command = new CommandDefinition(@"
            INSERT INTO connector.persons (id, name, language_id, username)
            VALUES (@id, @name, @language_id, @username)
            ON CONFLICT (id) DO UPDATE SET
                name = EXCLUDED.name,
                language_id = EXCLUDED.language_id,
                username = EXCLUDED.username;",
            new
            {
                id = person.Id,
                name = person.Name,
                language_id = person.LanguageId.Value,
                username = person.Username
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = new NpgsqlConnection(_connectionString);

        await connection.ExecuteAsync(command);
    }
}