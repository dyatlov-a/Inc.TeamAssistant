using Inc.TeamAssistant.Tenants.Model.Commands.CreateRoom;
using Inc.TeamAssistant.Tenants.Model.Commands.UpdateRoom;
using Inc.TeamAssistant.Tenants.Model.Queries.GetAvailableRooms;
using Inc.TeamAssistant.Tenants.Model.Queries.GetRoom;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface ITenantService
{
    Task<GetAvailableRoomsResult> GetAvailableRooms(Guid? id, CancellationToken token = default);
    
    Task<GetRoomResult> GetRoom(Guid id, CancellationToken token = default);
    
    Task<CreateRoomResult> CreateRoom(CreateRoomCommand command, CancellationToken token = default);
    
    Task UpdateRoom(UpdateRoomCommand command, CancellationToken token = default);

    Task RemoveRoom(Guid teamId, CancellationToken token = default);
}