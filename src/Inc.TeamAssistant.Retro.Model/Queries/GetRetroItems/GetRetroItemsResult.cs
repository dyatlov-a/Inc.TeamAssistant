namespace Inc.TeamAssistant.Retro.Model.Queries.GetRetroItems;

public sealed record GetRetroItemsResult(IReadOnlyCollection<RetroItemDto> Items);