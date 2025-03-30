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

    public async Task<Calendar?> Find(Guid botId, CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT
                c.id AS id,
                c.owner_id AS ownerid,
                c.schedule AS schedule,
                c.weekends AS weekends,
                c.holidays AS holidays
            FROM generic.calendars AS c
            JOIN connector.bots AS b ON b.calendar_id = c.id
            WHERE b.id = @bot_id;
            """,
            new { bot_id = botId },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var calendar = await connection.QuerySingleOrDefaultAsync<Calendar>(command);
        return calendar;
    }
}