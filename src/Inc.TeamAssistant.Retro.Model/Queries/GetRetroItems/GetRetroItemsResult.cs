using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Model.Common;

namespace Inc.TeamAssistant.Retro.Model.Queries.GetRetroItems;

public sealed record GetRetroItemsResult(IReadOnlyCollection<RetroItemDto> Items)
    : IWithEmpty<GetRetroItemsResult>
{
    public static GetRetroItemsResult Empty { get; } = new([]);
}