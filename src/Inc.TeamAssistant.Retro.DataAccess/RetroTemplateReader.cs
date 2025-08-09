using Dapper;
using Inc.TeamAssistant.Primitives.DataAccess;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Retro.DataAccess;

internal sealed class RetroTemplateReader : IRetroTemplateReader
{
    private readonly IConnectionFactory _connectionFactory;

    public RetroTemplateReader(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }
    
    public async Task<IReadOnlyCollection<RetroColumn>> GetColumns(Guid templateId, CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT
                c.id AS id,
                c.name AS name,
                c.position AS position,
                c.color AS color,
                c.description AS description
            FROM retro.columns AS c
            WHERE c.template_id = @template_id;
            """,
            new
            {
                template_id = templateId
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();
        
        var columns = await connection.QueryAsync<RetroColumn>(command);
        
        return columns.ToArray();
    }
}