using Dapper;
using Inc.TeamAssistant.Primitives.DataAccess;
using Inc.TeamAssistant.Primitives.Integrations;

namespace Inc.TeamAssistant.Connector.DataAccess;

internal sealed class IntegrationsAccessor : IIntegrationsAccessor
{
    private readonly IConnectionFactory _connectionFactory;
    
    public IntegrationsAccessor(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }
    
    public async Task<IntegrationContext?> Find(string accessToken, string projectKey, CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accessToken);
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        
        var command = new CommandDefinition(
            """
            SELECT
                t.id AS teamid,
                t.properties AS teamproperties,
                t.bot_id AS botid,
                t.owner_id AS ownerid,
                t.chat_id AS chatid
            FROM connector.teams AS t
            WHERE t.properties ->> 'accessToken' = @access_token AND t.properties ->> 'projectKey' = @project_key
            """,
            new
            {
                access_token = accessToken,
                project_key = projectKey
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        return connection.QuerySingleOrDefault<IntegrationContext>(command);
    }
}