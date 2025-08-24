using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Tenants.Model.Queries.GetRoomHistory;

public sealed record GetRoomHistoryResult(IReadOnlyCollection<RoomEntryDto> Items)
    : IWithEmpty<GetRoomHistoryResult>
{
    public static GetRoomHistoryResult Empty { get; } = new(Items: []);
}