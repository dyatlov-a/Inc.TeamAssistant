using System.Text.Json;
using Dapper;
using Inc.TeamAssistant.Primitives.DataAccess;
using Inc.TeamAssistant.Tenants.Application.Contracts;

namespace Inc.TeamAssistant.Tenants.DataAccess;

internal sealed class RoomPropertiesProvider : IRoomPropertiesProvider
{
    private readonly IConnectionFactory _connectionFactory;

    public RoomPropertiesProvider(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }
    
    public async Task<T> Get<T>(Guid roomId, CancellationToken token)
        where T : class, new()
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

        var properties = !string.IsNullOrWhiteSpace(data)
            ? JsonSerializer.Deserialize<T>(data)
            : null;

        return properties ?? new T();
    }

    public async Task Set<T>(Guid roomId, T properties, CancellationToken token)
        where T : class, new()
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