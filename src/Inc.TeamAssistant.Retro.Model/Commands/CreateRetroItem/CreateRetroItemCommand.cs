using MediatR;

namespace Inc.TeamAssistant.Retro.Model.Commands.CreateRetroItem;

public sealed record CreateRetroItemCommand(Guid TeamId, int Type, string? Text)
    : IRequest<CreateRetroItemResult>;