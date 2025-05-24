using MediatR;

namespace Inc.TeamAssistant.Retro.Model.Commands.CreateRetroItem;

public sealed record CreateRetroItemCommand(Guid TeamId, Guid ColumnId, string? Text)
    : IRequest<CreateRetroItemResult>;