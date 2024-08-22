using Dapper;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.DataAccess;

namespace Inc.TeamAssistant.Appraiser.DataAccess;

internal sealed class EstimatesStatsProvider : IPersonStatsProvider
{
    private readonly IConnectionFactory _connectionFactory;

    public EstimatesStatsProvider(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public string FeatureName => "Estimates";
    
    public async Task<IReadOnlyDictionary<long, int>> GetStats(
        IReadOnlyCollection<long> personIds,
        DateTimeOffset from,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(personIds);

        var command = new CommandDefinition(
            """
            SELECT
                sfe.participant_id AS personid,
                count(*) AS eventscount
            FROM appraiser.story_for_estimates AS sfe
            JOIN appraiser.stories AS s ON sfe.story_id = s.id
            WHERE s.created > @from AND sfe.participant_id = ANY(@person_ids) AND sfe.value > 0
            GROUP BY sfe.participant_id;
            """,
            new
            {
                person_ids = personIds,
                from
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();

        var results = await connection.QueryAsync<(long PersonId, int EventsCount)>(command);

        return results.ToDictionary();
    }
}