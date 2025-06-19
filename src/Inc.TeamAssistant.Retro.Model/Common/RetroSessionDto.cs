namespace Inc.TeamAssistant.Retro.Model.Common;

public sealed record RetroSessionDto(
    Guid Id,
    Guid RoomId,
    DateTimeOffset Created,
    string State);