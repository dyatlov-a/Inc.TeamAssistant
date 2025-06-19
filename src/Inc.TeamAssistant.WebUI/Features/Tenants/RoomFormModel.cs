using Inc.TeamAssistant.Tenants.Model.Commands.CreateRoom;
using Inc.TeamAssistant.Tenants.Model.Commands.UpdateRoom;
using Inc.TeamAssistant.Tenants.Model.Queries.Common;

namespace Inc.TeamAssistant.WebUI.Features.Tenants;

public sealed class RoomFormModel
{
    public string Name { get; set; } = string.Empty;

    public RoomFormModel Apply(RoomDto room)
    {
        ArgumentNullException.ThrowIfNull(room);
        
        Name = room.Name;

        return this;
    }

    public RoomFormModel Clear()
    {
        Name = string.Empty;
        
        return this;
    }

    public CreateRoomCommand ToCommand() => new(Name);
    public UpdateRoomCommand ToCommand(Guid roomId) => new(roomId, Name);
}