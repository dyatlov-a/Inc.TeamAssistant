namespace Inc.TeamAssistant.Retro.Model.Common;

public sealed record RetroItemDto(
    Guid Id,
    Guid RoomId,
    Guid ColumnId,
    int Position,
    string? Text,
    long OwnerId,
    Guid? ParentId,
    int Votes);