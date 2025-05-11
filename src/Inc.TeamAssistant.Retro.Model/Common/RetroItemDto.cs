namespace Inc.TeamAssistant.Retro.Model.Common;

public sealed record RetroItemDto(
    Guid Id,
    Guid TeamId,
    DateTimeOffset Created,
    string Type,
    string? Text,
    long OwnerId);