namespace Inc.TeamAssistant.Tenants.Model.Queries.GetRoomHistory;

public sealed record RoomEntryDto(
    Guid Id,
    string Type,
    DateTimeOffset Date);