using Dapper;
using Inc.TeamAssistant.Primitives.DataAccess;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Retro.DataAccess;

internal sealed class ActionItemRepository : IActionItemRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public ActionItemRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }
    
    public async Task<ActionItem?> Find(Guid id, CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT
                ai.id AS id,
                ai.retro_item_id AS retroitemid,
                ai.created AS created,
                ai.text AS text,
                ai.state AS state
            FROM retro.action_items AS ai
            WHERE ai.id = @id;
            """,
            new
            {
                id = id
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();
        
        var actionItem = await connection.QuerySingleOrDefaultAsync<ActionItem>(command);
        
        return actionItem;
    }

    public async Task Upsert(ActionItem item, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(item);
        
        var command = new CommandDefinition(
            """
            INSERT INTO retro.action_items (
                id,
                retro_item_id,
                created,
                text,
                state)
            VALUES (
                @id,
                @retro_item_id,
                @created,
                @text,
                @state)
            ON CONFLICT (id)
            DO UPDATE SET 
                retro_item_id = EXCLUDED.retro_item_id,
                created = EXCLUDED.created,
                text = EXCLUDED.text,
                state = EXCLUDED.state;
            """,
            new
            {
                id = item.Id,
                retro_item_id = item.RetroItemId,
                created = item.Created,
                text = item.Text,
                state = item.State
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();
        
        await connection.ExecuteAsync(command);
    }

    public async Task Remove(ActionItem item, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(item);
        
        var command = new CommandDefinition(
            """
            DELETE FROM retro.action_items AS ai
            WHERE ai.id = @item_id;
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