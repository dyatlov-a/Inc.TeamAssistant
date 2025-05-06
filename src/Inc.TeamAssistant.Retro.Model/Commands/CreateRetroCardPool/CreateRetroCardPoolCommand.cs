using MediatR;

namespace Inc.TeamAssistant.Retro.Model.Commands.CreateRetroCardPool;

public sealed record CreateRetroCardPoolCommand(string Name)
    : IRequest<CreateRetroCardPoolResult>;