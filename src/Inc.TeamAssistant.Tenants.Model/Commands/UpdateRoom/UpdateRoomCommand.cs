using MediatR;

namespace Inc.TeamAssistant.Tenants.Model.Commands.UpdateRoom;

public sealed record UpdateRoomCommand(Guid Id, string Name)
    : IRequest;