using Dapper;
using Inc.TeamAssistant.CheckIn.Application.Contracts;
using Inc.TeamAssistant.CheckIn.Domain;
using Inc.TeamAssistant.Primitives.DataAccess;

namespace Inc.TeamAssistant.CheckIn.DataAccess;

internal sealed class LocationsRepository : ILocationsRepository
{
    private readonly IConnectionFactory _connectionFactory;
    
    public LocationsRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<IReadOnlyCollection<Map>> GetByBot(Guid botId, CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT
                id AS id,
                chat_id AS chatid,
                bot_id AS botid,
                name AS name
            FROM maps.maps
            WHERE bot_id = @bot_id;",
            new { bot_id = botId },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();
        
        var results = await connection.QueryAsync<Map>(command);

        return results.ToArray();
    }

    public async Task<Map?> Find(long chatId, CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT
                id AS id,
                chat_id AS chatid,
                bot_id AS botid,
                name AS name
            FROM maps.maps
            WHERE chat_id = @chat_id;",
            new { chat_id = chatId },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        return await connection.QuerySingleOrDefaultAsync<Map>(command);
    }

    public async Task<IReadOnlyCollection<LocationOnMap>> GetLocations(Guid mapId, CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT
                l.id AS id,
                l.map_id AS mapid,
                l.user_id AS userid,
                l.display_name AS displayname,
                l.longitude AS longitude,
                l.latitude AS latitude,
                l.created AS created,
                m.id AS id,
                m.chat_id AS chatid,
                m.bot_id AS botid,
                m.name AS name
            FROM maps.locations AS l
            JOIN maps.maps AS m ON m.id = l.map_id
            WHERE l.map_id = @map_id;",
            new { map_id = mapId },
            flags: CommandFlags.Buffered,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var results = await connection.QueryAsync<LocationOnMap, Map, LocationOnMap>(
            command,
            (l, m) => l.SetMap(m),
            splitOn: "id");

        return results.ToArray();
    }

    public async Task Insert(LocationOnMap locationOnMap, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(locationOnMap);

        var upsertMap = new CommandDefinition(@"
            INSERT INTO maps.maps (id, chat_id, bot_id, name)
            VALUES (@id, @chat_id, @bot_id, @name)
            ON CONFLICT (id) DO UPDATE SET
                chat_id = excluded.chat_id,
                bot_id = excluded.bot_id,
                name = excluded.name;",
            new
            {
                id = locationOnMap.Map.Id,
                chat_id = locationOnMap.Map.ChatId,
                bot_id = locationOnMap.Map.BotId,
                name = locationOnMap.Map.Name
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        var upsertLocation = new CommandDefinition(@"
            INSERT INTO maps.locations (id, map_id, user_id, display_name, longitude, latitude, created)
            VALUES (@id, @map_id, @user_id, @display_name, @longitude, @latitude, @created)
            ON CONFLICT DO NOTHING;",
            new
            {
                id = locationOnMap.Id,
                map_id = locationOnMap.Map.Id,
                user_id = locationOnMap.UserId,
                display_name = locationOnMap.DisplayName,
                longitude = locationOnMap.Longitude,
                latitude = locationOnMap.Latitude,
                created = locationOnMap.Created
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();
        await connection.OpenAsync(token);
        await using var transaction = await connection.BeginTransactionAsync(token);

        await connection.ExecuteAsync(upsertMap);
        await connection.ExecuteAsync(upsertLocation);
        
        await transaction.CommitAsync(token);
    }
}