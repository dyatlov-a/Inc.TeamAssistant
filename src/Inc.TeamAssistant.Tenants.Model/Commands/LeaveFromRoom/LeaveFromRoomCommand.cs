using MediatR;

namespace Inc.TeamAssistant.Tenants.Model.Commands.LeaveFromRoom;

public sealed record LeaveFromRoomCommand(string ConnectionId, Guid RoomId)
    : IRequest;