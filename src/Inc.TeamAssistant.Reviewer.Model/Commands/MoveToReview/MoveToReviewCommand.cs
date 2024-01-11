using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Model.Commands.MoveToReview;

public sealed record MoveToReviewCommand(
    MessageContext MessageContext,
    Guid TeamId,
    string Description)
    : IRequest<CommandResult>;