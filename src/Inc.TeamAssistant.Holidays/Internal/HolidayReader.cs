using Dapper;
using Inc.TeamAssistant.Holidays.Model;
using Inc.TeamAssistant.Primitives.DataAccess;

namespace Inc.TeamAssistant.Holidays.Internal;

internal sealed class HolidayReader : IHolidayReader
{
    private readonly IConnectionFactory _connectionFactory;
    
    public HolidayReader(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<Dictionary<DateOnly, HolidayType>> GetAll(CancellationToken token)
    {
        var command = new CommandDefinition(@"
SELECT
    date AS date,
    type AS type
FROM generic.holidays;",
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var results = await connection.QueryAsync<Holiday>(command);
        return results.ToDictionary(r => r.Date, r => r.Type);
    }
}