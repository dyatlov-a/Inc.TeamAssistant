using MediatR;

namespace Inc.TeamAssistant.Retro.Model.Commands.RemoveActionItem;

public sealed record RemoveActionItemCommand(Guid Id, Guid TeamId, string ConnectionId)
    : IRequest;