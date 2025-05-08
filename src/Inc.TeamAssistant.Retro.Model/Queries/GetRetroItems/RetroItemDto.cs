namespace Inc.TeamAssistant.Retro.Model.Queries.GetRetroItems;

public sealed record RetroItemDto(
    Guid Id,
    Guid TeamId,
    DateTimeOffset Created,
    int Type,
    string Text,
    long OwnerId);