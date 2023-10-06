using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Model.Commands.CreateTeam;

public sealed record CreateTeamCommand(
    long ChatId,
    string ChatName,
    string NextReviewerType,
    LanguageId PersonLanguageId)
    : IRequest;