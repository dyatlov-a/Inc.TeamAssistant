using MediatR;

namespace Inc.TeamAssistant.Retro.Model.Commands.ChangeActionItem;

public sealed record ChangeActionItemCommand(
    Guid Id,
    Guid RetroItemId,
    Guid RoomId,
    string Text,
    string State,
    bool Notify)
    : IRequest;