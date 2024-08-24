using Dapper;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.DataAccess;

namespace Inc.TeamAssistant.RandomCoffee.DataAccess;

internal sealed class RandomCoffeeStatsProvider : IPersonStatsProvider
{
    private readonly IConnectionFactory _connectionFactory;

    public RandomCoffeeStatsProvider(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public string FeatureName => "RandomCoffee";
    
    public async Task<IReadOnlyDictionary<long, int>> GetStats(
        IReadOnlyCollection<long> personIds,
        DateTimeOffset from,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(personIds);

        var command = new CommandDefinition(
            """
            WITH data_source AS (
            	SELECT p."FirstId" AS first_id, p."SecondId" AS second_id, h.excluded_person_id AS excluded_person_id
            	FROM random_coffee.history AS h
            	CROSS JOIN jsonb_to_recordset(h.pairs) AS p("FirstId" bigint, "SecondId" bigint)
            	WHERE h.created > @from)
            SELECT 
            	q.personid,
            	count(*) AS eventscount
            FROM (
            	SELECT d1.first_id AS personid
            	FROM data_source AS d1
            	UNION
            	SELECT d2.second_id AS personid
            	FROM data_source AS d2
            	UNION
            	SELECT d3.excluded_person_id AS personid
            	FROM data_source AS d3) AS q
            WHERE q.personid = ANY(@person_ids)
            GROUP BY q.personid;
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