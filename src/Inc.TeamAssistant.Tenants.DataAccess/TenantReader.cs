using Dapper;
using Inc.TeamAssistant.Primitives.DataAccess;
using Inc.TeamAssistant.Tenants.Application.Contracts;
using Inc.TeamAssistant.Tenants.Domain;

namespace Inc.TeamAssistant.Tenants.DataAccess;

internal sealed class TenantReader : ITenantReader
{
    private readonly IConnectionFactory _connectionFactory;

    public TenantReader(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<IReadOnlyCollection<Room>> GetAvailableRooms(
        Guid? teamId,
        long personId,
        CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT
                r.id AS id,
                r.name AS name,
                r.owner_id AS ownerid,
                r.properties AS properties,
                t.id AS id,
                t.name AS name,
                t.owner_id AS ownerid
            FROM tenants.rooms AS r
            JOIN tenants.tenants AS t ON t.id = r.tenant_id
            WHERE r.id = @team_id OR r.owner_id = @person_id OR t.owner_id = @person_id;
            """,
            new
            {
                team_id = teamId,
                person_id = personId
            },
            flags: CommandFlags.Buffered,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();

        var rooms = await connection.QueryAsync<Room, Tenant, Room>(command, (tt, t) => tt.MapTenant(t));

        return rooms.ToArray();
    }
}