using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Model.Common;

namespace Inc.TeamAssistant.Retro.Model.Queries.GetActionItems;

public sealed record GetActionItemsResult(
    long? FacilitatorId,
    IReadOnlyCollection<ActionItemDto> Items)
    : IWithEmpty<GetActionItemsResult>
{
    public static GetActionItemsResult Empty { get; } = new(FacilitatorId: null, Items: []);
}