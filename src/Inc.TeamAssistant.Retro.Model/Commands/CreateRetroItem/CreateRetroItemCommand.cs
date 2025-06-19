using MediatR;

namespace Inc.TeamAssistant.Retro.Model.Commands.CreateRetroItem;

public sealed record CreateRetroItemCommand(Guid RoomId, Guid ColumnId, string? Text)
    : IRequest<CreateRetroItemResult>;