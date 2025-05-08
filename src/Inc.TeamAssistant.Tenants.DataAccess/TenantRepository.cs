using Dapper;
using Inc.TeamAssistant.Primitives.DataAccess;
using Inc.TeamAssistant.Tenants.Application.Contracts;
using Inc.TeamAssistant.Tenants.Domain;

namespace Inc.TeamAssistant.Tenants.DataAccess;

internal sealed class TenantRepository : ITenantRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public TenantRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<Tenant?> FindTenant(long ownerId, CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT
                t.id AS id,
                t.name AS name,
                t.owner_id AS ownerid
            FROM tenants.tenants AS t
            WHERE t.owner_id = @owner_id;
            """,
            new
            {
                owner_id = ownerId
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var tenant = await connection.QuerySingleOrDefaultAsync<Tenant>(command);
        
        return tenant;
    }

    public async Task<Team?> FindTeam(Guid id, CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT
                tt.id AS id,
                tt.name AS name,
                tt.owner_id AS ownerid,
                t.id AS id,
                t.name AS name,
                t.owner_id AS ownerid
            FROM tenants.teams AS tt
            JOIN tenants.tenants AS t ON t.id = tt.tenant_id
            WHERE tt.id = @team_id;
            """,
            new
            {
                team_id = id
            },
            flags: CommandFlags.Buffered,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var teams = await connection.QueryAsync<Team, Tenant, Team>(command, (tt, t) => tt.MapTenant(t));
        
        return teams.SingleOrDefault();
    }

    public async Task Upsert(Team team, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(team);
        
        var tenantCommand = new CommandDefinition(
            """
            INSERT INTO tenants.tenants (
                id,
                name,
                owner_id)
            VALUES (
                @id,
                @name,
                @owner_id)
            ON CONFLICT (id) DO UPDATE SET
                name = EXCLUDED.name,
                owner_id = EXCLUDED.owner_id;
            """,
            new
            {
                id = team.Tenant.Id,
                name = team.Tenant.Name,
                owner_id = team.Tenant.OwnerId
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        var teamCommand = new CommandDefinition(
            """
            INSERT INTO tenants.teams (
                id,
                name,
                owner_id,
                tenant_id)
            VALUES (
                @id,
                @name,
                @owner_id,
                @tenant_id)
            ON CONFLICT (id) DO UPDATE SET
                name = EXCLUDED.name,
                owner_id = EXCLUDED.owner_id,
                tenant_id = EXCLUDED.tenant_id;
            """,
            new
            {
                id = team.Id,
                name = team.Name,
                owner_id = team.OwnerId,
                tenant_id = team.Tenant.Id
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();
        await connection.OpenAsync(token);
        await using var transaction = await connection.BeginTransactionAsync(token);
        
        await connection.ExecuteAsync(tenantCommand);
        await connection.ExecuteAsync(teamCommand);

        await transaction.CommitAsync(token);
    }
}