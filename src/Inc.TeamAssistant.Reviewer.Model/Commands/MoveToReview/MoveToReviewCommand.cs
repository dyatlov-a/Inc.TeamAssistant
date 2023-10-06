using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Users;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Model.Commands.MoveToReview;

public sealed record MoveToReviewCommand(
        Guid TeamId,
        LanguageId PersonLanguageId,
        long PersonId,
        string PersonFirstName,
        string Description,
        UserIdentity? TargetUser)
    : IRequest<MoveToReviewResult>;