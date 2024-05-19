using Dapper;
using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Primitives.DataAccess;

namespace Inc.TeamAssistant.Connector.DataAccess;

internal sealed class PhotosRepository : IPhotosRepository
{
    private readonly IConnectionFactory _connectionFactory;
    
    public PhotosRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }
    
    public async Task<IReadOnlyCollection<(long PersonId, Guid BotId)>> Get(
        DateTimeOffset fromDate,
        CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT DISTINCT
                tm.person_id AS personid,
                t.bot_id AS botid
            FROM connector.teammates AS tm
            JOIN connector.teams AS t ON tm.team_id = t.id
            LEFT JOIN connector.photos AS ph ON tm.person_id = ph.person_id
            WHERE ph.date < @from_date OR ph.date IS NULL;",
            new { from_date = fromDate },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();
        
        var results = await connection.QueryAsync<(long PersonId, Guid BotId)>(command);

        return results.ToArray();
    }

    public async Task<Photo?> Find(long personId, CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT
                id AS id,
                person_id AS personid,
                date AS date,
                data AS data
            FROM connector.photos AS ph
            WHERE ph.person_id = @person_id;",
            new { person_id = personId },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        return await connection.QuerySingleOrDefaultAsync<Photo>(command);
    }

    public async Task Upsert(Photo photo, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(photo);

        var command = new CommandDefinition(@"
            INSERT INTO connector.photos AS p (id, person_id, date, data)
            VALUES (@id, @person_id, @date, @data)
            ON CONFLICT (id) DO UPDATE SET
                person_id = EXCLUDED.person_id,
                date = EXCLUDED.date,
                data = EXCLUDED.data;",
            new
            {
                id = photo.Id,
                person_id = photo.PersonId,
                date = photo.Date,
                data = photo.Data
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();

        await connection.ExecuteAsync(command);
    }
}