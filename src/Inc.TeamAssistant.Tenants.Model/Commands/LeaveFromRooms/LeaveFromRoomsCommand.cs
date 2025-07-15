using MediatR;

namespace Inc.TeamAssistant.Tenants.Model.Commands.LeaveFromRooms;

public sealed record LeaveFromRoomsCommand(string ConnectionId)
    : IRequest<LeaveFromRoomsResult>;