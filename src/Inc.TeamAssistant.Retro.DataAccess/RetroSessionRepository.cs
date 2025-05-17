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
    
    public async Task Create(RetroSession retro, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(retro);
        
        var upsertRetroCommand = new CommandDefinition(
            """
            INSERT INTO retro.retro_sessions (
                id,
                team_id,
                created,
                state,
                facilitator_id)
            VALUES (
                @id,
                @team_id,
                @created,
                @state,
                @facilitator_id)
            ON CONFLICT (id) DO UPDATE SET
                team_id = EXCLUDED.team_id,
                created = EXCLUDED.created,
                state = EXCLUDED.state,
                facilitator_id = EXCLUDED.facilitator_id;
            """,
            new
            {
                id = retro.Id,
                team_id = retro.TeamId,
                created = retro.Created,
                state = retro.State,
                facilitator_id = retro.FacilitatorId
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        var attachItemsCommand = new CommandDefinition(
            """
            UPDATE retro.retro_items AS ri
            SET 
                retro_session_id = @retro_session_id
            WHERE ri.team_id = @team_id AND ri.retro_session_id IS NULL;
            """,
            new
            {
                retro_session_id = retro.Id,
                team_id = retro.TeamId
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();
        await connection.OpenAsync(token);
        await using var transaction = await connection.BeginTransactionAsync(token);
        
        await connection.ExecuteAsync(upsertRetroCommand);
        await connection.ExecuteAsync(attachItemsCommand);

        await transaction.CommitAsync(token);
    }
}