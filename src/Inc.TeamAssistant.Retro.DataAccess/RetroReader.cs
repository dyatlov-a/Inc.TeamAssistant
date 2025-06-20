using Dapper;
using Inc.TeamAssistant.Primitives.DataAccess;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Retro.DataAccess;

internal sealed class RetroReader : IRetroReader
{
    private readonly IConnectionFactory _connectionFactory;

    public RetroReader(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }
    
    public async Task<IReadOnlyCollection<RetroItem>> ReadRetroItems(
        Guid roomId,
        IReadOnlyCollection<RetroSessionState> states,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(states);
        
        var targetStates = states.Select(s => (int)s).ToArray();
        var command = new CommandDefinition(
            """
            SELECT
                ri.id AS id,
                ri.room_id AS roomid,
                ri.created AS created,
                ri.column_id AS columnid,
                ri.position AS position,
                ri.text AS text,
                ri.owner_id AS ownerid,
                ri.retro_session_id AS retrosessionid,
                ri.parent_id AS parentid,
                ri.votes AS votes
            FROM retro.retro_items AS ri
            LEFT JOIN retro.retro_sessions AS rs ON ri.retro_session_id = rs.id
            WHERE ri.room_id = @room_id AND (rs.id IS NULL OR rs.state = ANY(@states));
            """,
            new
            {
                room_id = roomId,
                states = targetStates
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var items = await connection.QueryAsync<RetroItem>(command);
        
        return items.ToArray();
    }

    public async Task<RetroSession?> FindSession(
        Guid roomId,
        IReadOnlyCollection<RetroSessionState> states,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(states);
        
        var targetStates = states.Select(s => (int)s).ToArray();
        var command = new CommandDefinition(
            """
            SELECT
                rs.id AS id,
                rs.room_id AS roomid,
                rs.created AS created,
                rs.state AS state
            FROM retro.retro_sessions AS rs
            WHERE rs.room_id = @room_id AND rs.state = ANY(@states);
            """,
            new
            {
                room_id = roomId,
                states = targetStates
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var result = await connection.QuerySingleOrDefaultAsync<RetroSession>(command);
        
        return result;
    }

    public async Task<IReadOnlyCollection<ActionItem>> ReadActionItems(Guid retroSessionId, CancellationToken token)
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
            JOIN retro.retro_items AS ri ON ai.retro_item_id = ri.id
            WHERE ri.retro_session_id = @retro_session_id;
            """,
            new
            {
                retro_session_id = retroSessionId
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var items = await connection.QueryAsync<ActionItem>(command);
        
        return items.ToArray();
    }
}