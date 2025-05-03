using Dapper;
using Inc.TeamAssistant.Constructor.Application.Contracts;
using Inc.TeamAssistant.Constructor.Model.Queries.GetFeatures;
using Inc.TeamAssistant.Primitives.DataAccess;

namespace Inc.TeamAssistant.Constructor.DataAccess;

internal sealed class FeatureReader : IFeatureReader
{
    private readonly IConnectionFactory _connectionFactory;

    public FeatureReader(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }
    
    public async Task<IReadOnlyCollection<FeatureDto>> GetAll(CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT
                f.id AS id,
                f.name AS name,
                f.properties AS properties
            FROM connector.features AS f
            ORDER BY f.position;
            """,
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var results = await connection.QueryAsync<FeatureDto>(command);
        return results.ToArray();
    }
}