using Dapper;
using Inc.TeamAssistant.Primitives.DataAccess;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Retro.DataAccess;

internal sealed class RetroSessionRepository : IRetroSessionRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public RetroSessionRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<RetroSession?> Find(Guid id, CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT
                rs.id AS id,
                rs.room_id AS roomid,
                rs.created AS created,
                rs.state AS state
            FROM retro.retro_sessions AS rs
            WHERE rs.id = @id;
            """,
            new
            {
                id = id
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();
        
        var retroSession = await connection.QuerySingleOrDefaultAsync<RetroSession>(command);
        
        return retroSession;
    }

    public async Task Create(RetroSession retro, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(retro);
        
        var upsertRetroCommand = new CommandDefinition(
            """
            INSERT INTO retro.retro_sessions (
                id,
                room_id,
                created,
                state)
            VALUES (
                @id,
                @room_id,
                @created,
                @state)
            ON CONFLICT (id) DO UPDATE SET
                room_id = EXCLUDED.room_id,
                created = EXCLUDED.created,
                state = EXCLUDED.state;
            """,
            new
            {
                id = retro.Id,
                room_id = retro.RoomId,
                created = retro.Created,
                state = retro.State
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        var removeEmptyItemsCommand = new CommandDefinition(
            """
            DELETE FROM retro.retro_items AS ri
            WHERE ri.room_id = @room_id AND COALESCE(trim(ri.text), '') = '';
            """,
            new
            {
                room_id = retro.RoomId
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        var attachItemsCommand = new CommandDefinition(
            """
            UPDATE retro.retro_items AS ri
            SET 
                retro_session_id = @retro_session_id
            WHERE ri.room_id = @room_id AND ri.retro_session_id IS NULL;
            """,
            new
            {
                retro_session_id = retro.Id,
                room_id = retro.RoomId
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();
        await connection.OpenAsync(token);
        await using var transaction = await connection.BeginTransactionAsync(token);
        
        await connection.ExecuteAsync(upsertRetroCommand);
        await connection.ExecuteAsync(removeEmptyItemsCommand);
        await connection.ExecuteAsync(attachItemsCommand);

        await transaction.CommitAsync(token);
    }

    public async Task Update(RetroSession retro, IReadOnlyCollection<VoteTicket> voteTickets, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(retro);
        ArgumentNullException.ThrowIfNull(voteTickets);

        var itemId = new List<Guid>();
        var votes = new List<int>();
        
        foreach (var ticketsByItem in voteTickets.GroupBy(t => t.ItemId))
        {
            itemId.Add(ticketsByItem.Key);
            votes.Add(ticketsByItem.Sum(t => t.Vote));
        }
        
        var commandSession = new CommandDefinition(
            """
            UPDATE retro.retro_sessions AS rs
            SET
                state = @state
            WHERE rs.id = @id;
            """,
            new
            {
                id = retro.Id,
                state = retro.State
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        var commandItems = new CommandDefinition(
            """
            UPDATE retro.retro_items AS ri
            SET
                votes = v.votes
            FROM UNNEST(@item_id, @votes) AS v(item_id, votes)
            WHERE ri.id = v.item_id;
            """,
            new
            {
                item_id = itemId,
                votes = votes
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();
        await connection.OpenAsync(token);
        await using var transaction = await connection.BeginTransactionAsync(token);
        
        await connection.ExecuteAsync(commandSession);

        if (itemId.Any())
            await connection.ExecuteAsync(commandItems);
        
        await transaction.CommitAsync(token);
    }
}