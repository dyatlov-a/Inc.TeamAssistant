using MediatR;

namespace Inc.TeamAssistant.Retro.Model.Commands.UpdateRetroItem;

public sealed record UpdateRetroItemCommand(Guid Id, int Type, string Text)
    : IRequest;