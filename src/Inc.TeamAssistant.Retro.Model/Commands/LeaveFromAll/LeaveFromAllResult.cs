namespace Inc.TeamAssistant.Retro.Model.Commands.LeaveFromAll;

public sealed record LeaveFromAllResult(IReadOnlyCollection<Guid> RoomIds);