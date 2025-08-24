using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Model.Common;

namespace Inc.TeamAssistant.Retro.Model.Queries.GetRetroHistory;

public sealed record GetRetroHistoryResult(
    RetroSessionDto Session,
    IReadOnlyCollection<RetroItemDto> Items,
    IReadOnlyCollection<ActionItemDto> ActionItems,
    IReadOnlyCollection<RetroColumnDto> Columns,
    IReadOnlyCollection<int> Assessments)
    : IWithEmpty<GetRetroHistoryResult>
{
    public static GetRetroHistoryResult Empty { get; } = new(
        Session: new RetroSessionDto(
            Id: Guid.Empty,
            RoomId: Guid.Empty,
            Created: DateTimeOffset.MinValue,
            State: string.Empty),
        Items: [],
        ActionItems: [],
        Columns: [],
        Assessments: []);
}