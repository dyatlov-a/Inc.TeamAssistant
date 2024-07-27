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
        var command = new CommandDefinition(@"
            SELECT
                p.id AS id,
                p.name AS name,
                p.username AS username
            FROM connector.persons AS p
            WHERE p.id = @person_id;",
            new { person_id = personId },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        return await connection.QuerySingleOrDefaultAsync<Person>(command);
    }

    public async Task<Person?> Find(string username, CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT
                p.id AS id,
                p.name AS name,
                p.username AS username
            FROM connector.persons AS p
            WHERE LOWER(p.username) = @username;",
            new { username = username.ToLowerInvariant() },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();

        return await connection.QuerySingleOrDefaultAsync<Person>(command);
    }
    
    public async Task<IReadOnlyCollection<Person>> GetTeammates(
        Guid teamId,
        DateTimeOffset now,
        CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT
                p.id AS id,
                p.name AS name,
                p.username AS username
            FROM connector.persons AS p
            JOIN connector.teammates AS tm ON p.id = tm.person_id AND (tm.leave_until IS NULL OR tm.leave_until < @now)
            WHERE tm.team_id = @team_id;",
            new
            {
                team_id = teamId,
                now
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();

        var persons = await connection.QueryAsync<Person>(command);
        return persons.ToArray();
    }

    public async Task Upsert(Person person, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(person);

        var command = new CommandDefinition(@"
            INSERT INTO connector.persons AS p (id, name, username)
            VALUES (@id, @name, @username)
            ON CONFLICT (id) DO UPDATE SET
                name = EXCLUDED.name,
                username = EXCLUDED.username;",
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

    public async Task LeaveFromTeam(Guid teamId, long personId, CancellationToken token)
    {
        var command = new CommandDefinition(@"
            DELETE FROM connector.teammates
            WHERE person_id = @person_id AND team_id = @team_id;",
            new
            {
                team_id = teamId,
                person_id = personId
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();

        await connection.ExecuteAsync(command);
    }

    public async Task LeaveFromTeam(Guid teamId, long personId, DateTimeOffset? until, CancellationToken token)
    {
        var command = new CommandDefinition(@"
            UPDATE connector.teammates
            SET leave_until = @leave_until
            WHERE person_id = @person_id AND team_id = @team_id;",
            new
            {
                team_id = teamId,
                person_id = personId,
                leave_until = until
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();

        await connection.ExecuteAsync(command);
    }
    
    public async Task<Guid> GetBotId(
        long personId,
        CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT
                t.bot_id AS botid
            FROM connector.teammates AS tm
            JOIN connector.teams AS t ON tm.team_id = t.id
            WHERE tm.person_id = @person_id
            LIMIT 1;",
            new { person_id = personId },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();
        
        return await connection.QuerySingleOrDefaultAsync<Guid>(command);
    }
}