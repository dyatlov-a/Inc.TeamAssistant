using MediatR;

namespace Inc.TeamAssistant.Retro.Model.Commands.RemoveRetroItem;

public sealed record RemoveRetroItemCommand(Guid Id)
    : IRequest;