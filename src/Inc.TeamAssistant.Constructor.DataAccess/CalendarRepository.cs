using Dapper;
using Inc.TeamAssistant.Constructor.Application.Contracts;
using Inc.TeamAssistant.Holidays.Model;
using Inc.TeamAssistant.Primitives.DataAccess;

namespace Inc.TeamAssistant.Constructor.DataAccess;

internal sealed class CalendarRepository : ICalendarRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public CalendarRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }
    
    public async Task<Calendar?> FindByOwner(long ownerId, CancellationToken token)
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
            WHERE c.owner_id = @owner_id;
            """,
            new { owner_id = ownerId },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var calendar = await connection.QuerySingleOrDefaultAsync<Calendar>(command);
        return calendar;
    }
}