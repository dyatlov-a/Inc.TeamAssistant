namespace Inc.TeamAssistant.Tenants.Model.Commands.LeaveFromRooms;

public sealed record LeaveFromRoomsResult(IReadOnlyCollection<Guid> RoomIds);