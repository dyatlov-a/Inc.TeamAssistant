using MediatR;

namespace Inc.TeamAssistant.Retro.Model.Commands.ChangeActionItem;

public sealed record ChangeActionItemCommand(
    Guid Id,
    Guid RetroItemId,
    Guid? TeamIdForNotify,
    string Text,
    string? State)
    : IRequest;