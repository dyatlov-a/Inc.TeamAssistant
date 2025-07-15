using System.Text.Json;
using Dapper;
using Inc.TeamAssistant.Primitives.DataAccess;
using Inc.TeamAssistant.Primitives.Features.Rooms;
using Inc.TeamAssistant.Tenants.Application.Contracts;
using Inc.TeamAssistant.Tenants.Domain;

namespace Inc.TeamAssistant.Tenants.DataAccess;

internal sealed class RoomPropertiesProvider : IRoomPropertiesProvider
{
    private readonly IConnectionFactory _connectionFactory;

    public RoomPropertiesProvider(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }
    
    public async Task<RoomProperties> Get(Guid roomId, CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT r.properties
            FROM tenants.rooms AS r
            WHERE r.id = @room_id;
            """, new
            {
                room_id = roomId
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();
        
        var data = await connection.QuerySingleOrDefaultAsync<string>(command);

        var properties = JsonSerializer.Deserialize<RoomProperties>(data!)!;

        return properties;
    }

    public async Task Set(Guid roomId, RoomProperties properties, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(properties);
        
        var command = new CommandDefinition(
            """
            UPDATE tenants.rooms AS r
            SET properties = @properties::JSONB
            WHERE r.id = @room_id;
            """, new
            {
                room_id = roomId,
                properties = JsonSerializer.Serialize(properties)
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        await connection.ExecuteAsync(command);
    }
}