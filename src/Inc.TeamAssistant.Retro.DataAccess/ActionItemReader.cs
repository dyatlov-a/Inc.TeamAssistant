using Dapper;
using Inc.TeamAssistant.Primitives.DataAccess;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Retro.DataAccess;

internal sealed class ActionItemReader : IActionItemReader
{
    private readonly IConnectionFactory _connectionFactory;

    public ActionItemReader(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }
    
    public async Task<IReadOnlyCollection<ActionItem>> Read(
        Guid roomId,
        ActionItemState state,
        int offset,
        int limit,
        CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT
                ai.id AS id,
                ai.retro_item_id AS retroitemid,
                ai.created AS created,
                ai.text AS text,
                ai.state AS state,
                ai.modified as modified
            FROM retro.action_items AS ai
            JOIN retro.retro_items AS ri ON ai.retro_item_id = ri.id
            WHERE ri.room_id = @room_id AND ai.state = @state
            ORDER BY COALESCE(ai.modified, ai.created) DESC
            OFFSET @offset ROWS
            FETCH NEXT @limit ROWS ONLY;
            """,
            new
            {
                room_id = roomId,
                state = (int)state,
                offset = offset,
                limit = limit
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var items = await connection.QueryAsync<ActionItem>(command);
        
        return items.ToArray();
    }
}