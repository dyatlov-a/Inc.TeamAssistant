using MediatR;

namespace Inc.TeamAssistant.Tenants.Model.Commands.JoinToRoom;

public sealed record JoinToRoomCommand(string ConnectionId, Guid RoomId)
    : IRequest;