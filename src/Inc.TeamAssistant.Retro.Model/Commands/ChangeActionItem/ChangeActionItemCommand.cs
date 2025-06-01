using MediatR;

namespace Inc.TeamAssistant.Retro.Model.Commands.ChangeActionItem;

public sealed record ChangeActionItemCommand(
    Guid TeamId,
    Guid? Id,
    Guid RetroItemId,
    string Text)
    : IRequest;