using System.Text.Json;
using Dapper;
using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Primitives.DataAccess;

namespace Inc.TeamAssistant.Connector.DataAccess;

internal sealed class DashboardSettingsRepository : IDashboardSettingsRepository
{
    private readonly IConnectionFactory _connectionFactory;
    
    public DashboardSettingsRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }
    
    public async Task<DashboardSettings?> Find(long personId, Guid botId, CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT
                s.person_id AS personid,
                s.bot_id AS botid,
                s.widgets AS widgets
            FROM connector.dashboard_settings AS s
            WHERE s.person_id = @person_id AND s.bot_id = @bot_id
            """,
            new
            {
                person_id = personId,
                bot_id = botId
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();

        return await connection.QuerySingleOrDefaultAsync<DashboardSettings>(command);
    }

    public async Task Upsert(DashboardSettings settings, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(settings);

        var widgets = JsonSerializer.Serialize(settings.Widgets);
        var command = new CommandDefinition(
            """
            INSERT INTO connector.dashboard_settings (person_id, bot_id, widgets)
            VALUES (@person_id, @bot_id, @widgets::jsonb)
            ON CONFLICT (person_id, bot_id) DO UPDATE
            SET widgets = @widgets::jsonb
            """,
            new
            {
                person_id = settings.PersonId,
                bot_id = settings.BotId,
                widgets = widgets
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();
        
        await connection.ExecuteAsync(command);
    }
}