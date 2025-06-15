namespace Inc.TeamAssistant.Retro.Model.Queries.GetRetroState;

public sealed record RetroColumnDto(
    Guid Id,
    string Name,
    int Position,
    string Color,
    string? Description);