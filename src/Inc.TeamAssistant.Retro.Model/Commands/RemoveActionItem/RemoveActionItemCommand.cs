using MediatR;

namespace Inc.TeamAssistant.Retro.Model.Commands.RemoveActionItem;

public sealed record RemoveActionItemCommand(Guid Id, Guid RoomId, string ConnectionId)
    : IRequest;