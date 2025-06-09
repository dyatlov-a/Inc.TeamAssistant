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

    public async Task<Room?> FindRoom(Guid id, CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT
                r.id AS id,
                r.name AS name,
                r.owner_id AS ownerid,
                t.id AS id,
                t.name AS name,
                t.owner_id AS ownerid
            FROM tenants.rooms AS r
            JOIN tenants.tenants AS t ON t.id = r.tenant_id
            WHERE r.id = @team_id;
            """,
            new
            {
                team_id = id
            },
            flags: CommandFlags.Buffered,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var rooms = await connection.QueryAsync<Room, Tenant, Room>(command, (tt, t) => tt.MapTenant(t));
        
        return rooms.SingleOrDefault();
    }

    public async Task Upsert(Room room, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(room);
        
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
                id = room.Tenant.Id,
                name = room.Tenant.Name,
                owner_id = room.Tenant.OwnerId
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        var teamCommand = new CommandDefinition(
            """
            INSERT INTO tenants.rooms (
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
                id = room.Id,
                name = room.Name,
                owner_id = room.OwnerId,
                tenant_id = room.Tenant.Id
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

    public async Task Remove(Room room, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(room);
        
        var command = new CommandDefinition(
            """
            DELETE FROM tenants.rooms AS r
            WHERE r.id = @team_id;
            """,
            new
            {
                team_id = room.Id
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();
        
        await connection.ExecuteAsync(command);
    }
}