using MediatR;

namespace Inc.TeamAssistant.Reviewer.Model.Commands.UpdateTeammate;

public sealed record UpdateTeammateCommand(
    Guid TeamId,
    long PersonId,
    bool HasLeaveUntil,
    DateTimeOffset? LeaveUntil,
    bool? CanFinalize)
    : IRequest;