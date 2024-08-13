using System.Text.Json;
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

    public async Task<IReadOnlyCollection<Guid>> GetBotIds(Guid calendarId, CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT
                b.id AS id
            FROM connector.bots AS b
            WHERE b.calendar_id = @calendar_id;
            """,
            new { calendar_id = calendarId },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var botIds = await connection.QueryAsync<Guid>(command);
        return botIds.ToArray();
    }

    public async Task<Calendar> GetById(Guid id, CancellationToken token)
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
            WHERE c.id = @id;
            """,
            new { id },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var calendar = await connection.QuerySingleAsync<Calendar>(command);
        return calendar;
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

    public async Task Upsert(Calendar calendar, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(calendar);

        var schedule = calendar.Schedule is null
            ? null
            : JsonSerializer.Serialize(calendar.Schedule);
        var command = new CommandDefinition(
            """
            INSERT INTO generic.calendars (id, owner_id, schedule, weekends, holidays)
            VALUES (@id, @owner_id, @schedule::jsonb, @weekends::jsonb, @holidays::jsonb)
            ON CONFLICT (id) DO UPDATE SET
                owner_id = EXCLUDED.owner_id,
                schedule = EXCLUDED.schedule,
                weekends = EXCLUDED.weekends,
                holidays = EXCLUDED.holidays;
            """,
            new
            {
                id = calendar.Id,
                owner_id = calendar.OwnerId,
                schedule,
                weekends = JsonSerializer.Serialize(calendar.Weekends),
                holidays = JsonSerializer.Serialize(calendar.Holidays),
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();
        
        await connection.ExecuteAsync(command);
    }
}