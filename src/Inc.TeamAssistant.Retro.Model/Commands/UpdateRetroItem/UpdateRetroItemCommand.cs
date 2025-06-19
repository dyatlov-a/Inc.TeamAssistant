using MediatR;

namespace Inc.TeamAssistant.Retro.Model.Commands.UpdateRetroItem;

public sealed record UpdateRetroItemCommand(
    Guid Id,
    Guid ColumnId,
    int Position,
    string? Text,
    Guid? ParentId)
    : IRequest;