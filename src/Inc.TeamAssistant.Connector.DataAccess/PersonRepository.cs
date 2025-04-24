using Dapper;
using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.DataAccess;

namespace Inc.TeamAssistant.Connector.DataAccess;

internal sealed class PersonRepository : IPersonRepository
{
    private readonly IConnectionFactory _connectionFactory;
    
    public PersonRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }
    
    public async Task<Person?> Find(long personId, CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT
                p.id AS id,
                p.name AS name,
                p.username AS username
            FROM connector.persons AS p
            WHERE p.id = @person_id;
            """,
            new { person_id = personId },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        return await connection.QuerySingleOrDefaultAsync<Person>(command);
    }

    public async Task<Person?> Find(string username, CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT
                p.id AS id,
                p.name AS name,
                p.username AS username
            FROM connector.persons AS p
            WHERE LOWER(p.username) = @username;
            """,
            new { username = username.ToLowerInvariant() },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();

        return await connection.QuerySingleOrDefaultAsync<Person>(command);
    }

    public async Task Upsert(Person person, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(person);

        var command = new CommandDefinition(
            """
            INSERT INTO connector.persons (id, name, username)
            VALUES (@id, @name, @username)
            ON CONFLICT (id) DO UPDATE SET
                name = EXCLUDED.name,
                username = EXCLUDED.username;
            """,
            new
            {
                id = person.Id,
                name = person.Name,
                username = person.Username
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();

        await connection.ExecuteAsync(command);
    }
}