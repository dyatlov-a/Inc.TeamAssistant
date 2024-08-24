using Dapper;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.DataAccess;

namespace Inc.TeamAssistant.CheckIn.DataAccess;

internal sealed class CheckInStatsProvider : IPersonStatsProvider
{
    private readonly IConnectionFactory _connectionFactory;

    public CheckInStatsProvider(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public string FeatureName => "CheckIn";
    
    public async Task<IReadOnlyDictionary<long, int>> GetStats(
        IReadOnlyCollection<long> personIds,
        DateTimeOffset from,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(personIds);

        var command = new CommandDefinition(
            """
            SELECT
                l.user_id AS personid,
                count(*) AS eventscount
            FROM maps.locations AS l
            WHERE l.created > @from AND l.user_id = ANY(@person_ids)
            GROUP BY l.user_id;
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