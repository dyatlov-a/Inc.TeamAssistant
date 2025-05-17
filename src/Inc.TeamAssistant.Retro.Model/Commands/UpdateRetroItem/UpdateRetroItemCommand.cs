using MediatR;

namespace Inc.TeamAssistant.Retro.Model.Commands.UpdateRetroItem;

public sealed record UpdateRetroItemCommand(Guid Id, string? Text, Guid? ParentId)
    : IRequest;