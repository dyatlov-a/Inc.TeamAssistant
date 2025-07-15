using MediatR;

namespace Inc.TeamAssistant.Tenants.Model.Commands.ChangeTimer;

public sealed record ChangeTimerCommand(Guid RoomId, TimeSpan? Duration)
    : IRequest;