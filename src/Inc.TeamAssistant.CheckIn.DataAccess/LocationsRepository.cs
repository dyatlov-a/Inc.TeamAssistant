using Dapper;
using Inc.TeamAssistant.CheckIn.Application.Contracts;
using Inc.TeamAssistant.CheckIn.Domain;
using Npgsql;

namespace Inc.TeamAssistant.CheckIn.DataAccess;

internal sealed class LocationsRepository : ILocationsRepository
{
    private readonly string _connectionString;

    public LocationsRepository(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));

        _connectionString = connectionString;
    }

    public async Task<Map?> Find(long chatId, CancellationToken cancellationToken)
    {
        var command = new CommandDefinition(@"
SELECT id AS id, chat_id AS chatid
FROM maps.maps
WHERE chat_id = @chat_id;",
            new {chat_id = chatId},
            flags: CommandFlags.None,
            cancellationToken: cancellationToken);

        await using var connection = new NpgsqlConnection(_connectionString);

        return await connection.QuerySingleOrDefaultAsync<Map>(command);
    }

    public async Task<IReadOnlyCollection<LocationOnMap>> GetLocations(Guid mapId, CancellationToken cancellationToken)
    {
        var command = new CommandDefinition(@"
SELECT
    id AS id,
    map_id AS mapid,
    user_id AS userid,
    display_name AS displayname,
    longitude AS longitude,
    latitude AS latitude,
    created AS created
FROM maps.locations
WHERE map_id = @map_id;",
            new {map_id = mapId},
            flags: CommandFlags.None,
            cancellationToken: cancellationToken);

        await using var connection = new NpgsqlConnection(_connectionString);

        var results = await connection.QueryAsync<LocationOnMap>(command);

        return results.ToArray();
    }

    public async Task Insert(LocationOnMap locationOnMap, CancellationToken cancellationToken)
    {
        if (locationOnMap is null)
            throw new ArgumentNullException(nameof(locationOnMap));

        var command = new CommandDefinition(@"
INSERT INTO maps.maps (id, chat_id)
VALUES (@map_id, @chat_id)
ON CONFLICT (id) DO UPDATE SET
chat_id = excluded.chat_id;

INSERT INTO maps.locations (id, map_id, user_id, display_name, longitude, latitude, created, data)
VALUES (@id, @map_id, @user_id, @display_name, @longitude, @latitude, @created, @data::jsonb);",
            new
            {
                id = locationOnMap.Id,
                map_id = locationOnMap.Map.Id,
                chat_id = locationOnMap.Map.ChatId,
                user_id = locationOnMap.UserId,
                display_name = locationOnMap.DisplayName,
                longitude = locationOnMap.Longitude,
                latitude = locationOnMap.Latitude,
                created = locationOnMap.Created,
                data = locationOnMap.Data
            },
            flags: CommandFlags.None,
            cancellationToken: cancellationToken);

        await using var connection = new NpgsqlConnection(_connectionString);

        await connection.ExecuteAsync(command);
    }
}