namespace Inc.TeamAssistant.Retro.Model.Common;

public sealed record ActionItemDto(
    Guid Id,
    Guid RetroItemId,
    DateTimeOffset Created,
    string Text,
    string State,
    DateTimeOffset? Modified);