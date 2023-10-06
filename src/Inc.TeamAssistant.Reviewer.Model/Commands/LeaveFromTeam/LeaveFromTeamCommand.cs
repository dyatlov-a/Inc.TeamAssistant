using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Model.Commands.LeaveFromTeam;

public sealed record LeaveFromTeamCommand(
    Guid TeamId,
    long PersonId,
    string PersonFirstName,
    LanguageId PersonLanguageId)
    : IRequest;