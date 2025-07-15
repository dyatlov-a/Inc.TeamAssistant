using Inc.TeamAssistant.Tenants.Model.Commands.CreateRoom;
using Inc.TeamAssistant.Tenants.Model.Commands.UpdateRoom;
using Inc.TeamAssistant.Tenants.Model.Queries.GetAvailableRooms;
using Inc.TeamAssistant.Tenants.Model.Queries.GetRoom;
using Inc.TeamAssistant.Tenants.Model.Queries.GetRoomProperties;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface ITenantService
{
    Task<GetRoomPropertiesResult> GetRoomProperties(Guid roomId, CancellationToken token = default);
    
    Task<GetAvailableRoomsResult> GetAvailableRooms(Guid? roomId, CancellationToken token = default);
    
    Task<GetRoomResult> GetRoom(Guid roomId, CancellationToken token = default);
    
    Task<CreateRoomResult> CreateRoom(CreateRoomCommand command, CancellationToken token = default);
    
    Task UpdateRoom(UpdateRoomCommand command, CancellationToken token = default);

    Task RemoveRoom(Guid roomId, CancellationToken token = default);
}