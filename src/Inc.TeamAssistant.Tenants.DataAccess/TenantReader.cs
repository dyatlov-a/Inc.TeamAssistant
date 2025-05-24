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

    public async Task<IReadOnlyCollection<Team>> GetAvailableTeams(
        Guid? teamId,
        long personId,
        CancellationToken token)
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
            WHERE tt.id = @team_id OR tt.owner_id = @person_id OR t.owner_id = @person_id;
            """,
            new
            {
                team_id = teamId,
                person_id = personId
            },
            flags: CommandFlags.Buffered,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();

        var teams = await connection.QueryAsync<Team, Tenant, Team>(command, (tt, t) => tt.MapTenant(t));

        return teams.ToArray();
    }
}