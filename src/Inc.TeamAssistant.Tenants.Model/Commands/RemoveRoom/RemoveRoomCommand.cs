using MediatR;

namespace Inc.TeamAssistant.Tenants.Model.Commands.RemoveRoom;

public sealed record RemoveRoomCommand(Guid RoomId)
    : IRequest;