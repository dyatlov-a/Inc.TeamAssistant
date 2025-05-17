using Dapper;
using Inc.TeamAssistant.Primitives.DataAccess;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Retro.DataAccess;

internal sealed class RetroItemRepository : IRetroItemRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public RetroItemRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<RetroItem?> Find(Guid id, CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT
                ri.id AS id,
                ri.team_id AS teamid,
                ri.created AS created,
                ri.column_id AS columnid,
                ri.position AS position,
                ri.text AS text,
                ri.owner_id AS ownerid,
                ri.retro_session_id AS retrosessionid,
                ri.parent_id AS parentid
            FROM retro.retro_items AS ri
            WHERE ri.id = @id;

            SELECT
                rs.id AS id,
                rs.team_id AS teamid,
                rs.created AS created,
                rs.state AS state,
                rs.facilitator_id AS facilitatorid
            FROM retro.retro_items AS ri
            JOIN retro.retro_sessions AS rs ON ri.retro_session_id = rs.id
            WHERE ri.id = @id;

            SELECT
                ri.id AS id,
                ri.team_id AS teamid,
                ri.created AS created,
                ri.column_id AS columnid,
                ri.position AS position,
                ri.text AS text,
                ri.owner_id AS ownerid,
                ri.retro_session_id AS retrosessionid,
                ri.parent_id AS parentid
            FROM retro.retro_items AS ri
            WHERE ri.parent_id = @id;
            """,
            new
            {
                id = id
            },
            flags: CommandFlags.Buffered,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var query = await connection.QueryMultipleAsync(command);
        
        var item = await query.ReadSingleOrDefaultAsync<RetroItem>();
        var session = await query.ReadSingleOrDefaultAsync<RetroSession>();
        var children = await query.ReadAsync<RetroItem>();

        item?.MapRetroSession(session).MapChildren(children.ToArray());
        
        return item;
    }

    public async Task Upsert(RetroItem item, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(item);

        var id = new List<Guid>();
        var teamId = new List<Guid>();
        var created = new List<DateTimeOffset>();
        var columnId = new List<Guid>();
        var position = new List<int>();
        var text = new List<string?>();
        var ownerId = new List<long>();
        var retroSessionId = new List<Guid?>();
        var parentId = new List<Guid?>();

        foreach (var data in item.Children.Append(item))
        {
            id.Add(data.Id);
            teamId.Add(data.TeamId);
            created.Add(data.Created);
            columnId.Add(data.ColumnId);
            position.Add(data.Position);
            text.Add(data.Text);
            ownerId.Add(data.OwnerId);
            retroSessionId.Add(data.RetroSessionId);
            parentId.Add(data.ParentId);
        }
        
        var command = new CommandDefinition(
            """
            INSERT INTO retro.retro_items (
                id,
                team_id,
                created,
                column_id,
                position,
                text,
                owner_id,
                retro_session_id,
                parent_id)
            SELECT
                id,
                team_id,
                created,
                column_id,
                position,
                text,
                owner_id,
                retro_session_id,
                parent_id
            FROM UNNEST(@id, @team_id, @created, @column_id, @position, @text, @owner_id, @retro_session_id, @parent_id)
                AS i(id, team_id, created, column_id, position, text, owner_id, retro_session_id, parent_id)
            ON CONFLICT (id) DO UPDATE SET
                team_id = EXCLUDED.team_id,
                created = EXCLUDED.created,
                column_id = EXCLUDED.column_id,
                position = EXCLUDED.position,
                text = EXCLUDED.text,
                owner_id = EXCLUDED.owner_id,
                retro_session_id = EXCLUDED.retro_session_id,
                parent_id = EXCLUDED.parent_id;
            """,
            new
            {
                id = id,
                team_id = teamId,
                created = created,
                column_id = columnId,
                position = position,
                text = text,
                owner_id = ownerId,
                retro_session_id = retroSessionId,
                parent_id = parentId
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();
        
        await connection.ExecuteAsync(command);
    }

    public async Task Remove(RetroItem item, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(item);
        
        var command = new CommandDefinition(
            """
            DELETE FROM retro.retro_items AS ri
            WHERE ri.id = @item_id;
            """,
            new
            {
                item_id = item.Id
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();
        
        await connection.ExecuteAsync(command);
    }
}