namespace Inc.TeamAssistant.Retro.Model.Common;

public sealed record RetroColumnDto(
    Guid Id,
    string Name,
    int Position,
    string Color,
    string? Description);