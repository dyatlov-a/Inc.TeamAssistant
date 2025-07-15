namespace Inc.TeamAssistant.Primitives.Features.Rooms;

public interface IRoomPropertiesProvider
{
    Task<RoomProperties> Get(Guid roomId, CancellationToken token);
    
    Task Set(Guid roomId, RoomProperties properties, CancellationToken token);
}