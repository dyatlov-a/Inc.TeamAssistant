using MediatR;

namespace Inc.TeamAssistant.Tenants.Model.Commands.CreateRoom;

public sealed record CreateRoomCommand(string Name)
    : IRequest<CreateRoomResult>;