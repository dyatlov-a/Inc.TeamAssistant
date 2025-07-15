using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Model.Common;

namespace Inc.TeamAssistant.Retro.Model.Queries.GetActionItemsHistory;

public sealed record GetActionItemsHistoryResult(IReadOnlyCollection<ActionItemDto> Items)
    : IWithEmpty<GetActionItemsHistoryResult>
{
    public static GetActionItemsHistoryResult Empty { get; } = new(Items: []);
}