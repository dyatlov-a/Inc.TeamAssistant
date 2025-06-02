using Inc.TeamAssistant.Retro.Model.Common;

namespace Inc.TeamAssistant.Retro.Model.Queries.GetActionItems;

public sealed record GetActionItemsResult(IReadOnlyCollection<ActionItemDto> Items);