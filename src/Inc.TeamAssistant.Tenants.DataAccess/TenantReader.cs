using Dapper;
using Inc.TeamAssistant.Primitives.DataAccess;
using Inc.TeamAssistant.Tenants.Application.Contracts;
using Inc.TeamAssistant.Tenants.Domain;
using Inc.TeamAssistant.Tenants.Model.Queries.GetRoomHistory;
using Inc.TeamAssistant.Tenants.Model.Queries.GetRoomProperties;

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

    public async Task<IReadOnlyCollection<TemplateDto>> GetRetroTemplates(CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT
                t.id AS id,
                t.name AS name
            FROM retro.templates AS t;
            """,
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();
        
        var templates = await connection.QueryAsync<TemplateDto>(command);
        
        return templates.ToArray();
    }

    public async Task<IReadOnlyCollection<TemplateDto>> GetSurveyTemplates(CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT
                t.id AS id,
                t.name AS name
            FROM survey.templates AS t;
            """,
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();
        
        var templates = await connection.QueryAsync<TemplateDto>(command);
        
        return templates.ToArray();
    }

    public async Task<IReadOnlyCollection<RoomEntryDto>> GetRoomHistory(
        Guid roomId,
        DateTimeOffset from,
        CancellationToken token)
    {
        var surveyCompleted = 2;
        var retroCompleted = 4;
        var command = new CommandDefinition(
            """
            SELECT
                s.id AS id,
                'Survey' AS type,
                s.created AS date
            FROM survey.surveys AS s
            WHERE s.room_id = @room_id AND s.state = @survey_completed AND s.created > @from
            UNION
            SELECT
                r.id AS id,
                'Retro' AS type,
                r.created AS date
            FROM retro.retro_sessions AS r
            WHERE r.room_id = @room_id AND r.state = @retro_completed AND r.created > @from
            """,
            new
            {
                room_id = roomId,
                from = from.UtcDateTime,
                survey_completed = surveyCompleted,
                retro_completed = retroCompleted
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();
        
        var items = await connection.QueryAsync<RoomEntryDto>(command);
        
        return items.ToArray();
    }
}