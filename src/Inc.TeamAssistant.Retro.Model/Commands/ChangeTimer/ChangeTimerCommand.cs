using MediatR;

namespace Inc.TeamAssistant.Retro.Model.Commands.ChangeTimer;

public sealed record ChangeTimerCommand(Guid TeamId, TimeSpan? Duration)
    : IRequest;