namespace Inc.TeamAssistant.Retro.Model.Common;

public sealed record RetroItemDto(
    Guid Id,
    Guid TeamId,
    DateTimeOffset Created,
    int Type,
    string? Text,
    long OwnerId);