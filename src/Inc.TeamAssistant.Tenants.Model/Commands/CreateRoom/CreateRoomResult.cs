using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Tenants.Model.Commands.CreateRoom;

public sealed record CreateRoomResult(Guid RoomId)
    : IWithEmpty<CreateRoomResult>
{
    public static CreateRoomResult Empty { get; } = new(Guid.Empty);
}