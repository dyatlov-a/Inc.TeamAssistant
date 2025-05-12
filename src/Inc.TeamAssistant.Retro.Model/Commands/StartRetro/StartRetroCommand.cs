using MediatR;

namespace Inc.TeamAssistant.Retro.Model.Commands.StartRetro;

public sealed record StartRetroCommand(Guid TeamId)
    : IRequest<StartRetroResult>;