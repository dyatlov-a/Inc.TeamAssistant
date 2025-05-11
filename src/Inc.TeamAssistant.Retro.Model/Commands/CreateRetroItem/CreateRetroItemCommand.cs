using MediatR;

namespace Inc.TeamAssistant.Retro.Model.Commands.CreateRetroItem;

public sealed record CreateRetroItemCommand(Guid TeamId, string Type, string? Text)
    : IRequest<CreateRetroItemResult>;