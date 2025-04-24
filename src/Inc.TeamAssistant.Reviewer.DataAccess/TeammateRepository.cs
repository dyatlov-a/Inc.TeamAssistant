using Dapper;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.DataAccess;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.DataAccess;

internal sealed class TeammateRepository : ITeammateRepository
{
    private readonly IConnectionFactory _connectionFactory;
    
    public TeammateRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }
    
    public async Task<Teammate?> Find(TeammateKey key, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(key);
        
        var command = new CommandDefinition(
            """
            SELECT
                t.team_id AS teamid,
                t.person_id AS personid,
                t.leave_until AS leaveuntil,
                t.can_finalize AS canfinalize
            FROM connector.teammates AS t
            WHERE t.team_id = @team_id AND t.person_id = @person_id;
            """,
            new
            {
                team_id = key.TeamId,
                person_id = key.PersonId
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();

        return await connection.QuerySingleOrDefaultAsync<Teammate>(command);
    }
    
    public async Task RemoveFromTeam(TeammateKey key, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(key);
        
        var command = new CommandDefinition(
            """
            DELETE FROM connector.teammates
            WHERE person_id = @person_id AND team_id = @team_id;
            """,
            new
            {
                team_id = key.TeamId,
                person_id = key.PersonId
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();

        await connection.ExecuteAsync(command);
    }

    public async Task Upsert(Teammate teammate, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(teammate);
        
        var command = new CommandDefinition(
            """
            INSERT INTO connector.teammates (team_id, person_id, leave_until, can_finalize)
            VALUES (@team_id, @person_id, @leave_until, @can_finalize)
            ON CONFLICT (team_id, person_id) DO UPDATE SET
                leave_until = EXCLUDED.leave_until,
                can_finalize = EXCLUDED.can_finalize;
            """,
            new
            {
                team_id = teammate.TeamId,
                person_id = teammate.PersonId,
                leave_until = teammate.LeaveUntil,
                can_finalize = teammate.CanFinalize
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();

        await connection.ExecuteAsync(command);
    }
}