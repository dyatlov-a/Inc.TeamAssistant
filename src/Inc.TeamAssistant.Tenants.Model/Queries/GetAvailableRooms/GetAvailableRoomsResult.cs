using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Tenants.Model.Common;

namespace Inc.TeamAssistant.Tenants.Model.Queries.GetAvailableRooms;

public sealed record GetAvailableRoomsResult(IReadOnlyCollection<RoomDto> Rooms)
    : IWithEmpty<GetAvailableRoomsResult>
{
    public static GetAvailableRoomsResult Empty { get; } = new([]);
}