using MediatR;

namespace Inc.TeamAssistant.Retro.Model.Commands.UpdateRetroCardPool;

public sealed record UpdateRetroCardPoolCommand(Guid Id, string Name)
    : IRequest;