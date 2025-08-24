using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Tenants.Model.Common;

namespace Inc.TeamAssistant.Tenants.Model.Queries.GetRoom;

public sealed record GetRoomResult(RoomDto Room)
    : IWithEmpty<GetRoomResult>
{
    public static GetRoomResult Empty { get; } = new(new RoomDto(Guid.Empty, string.Empty));
}