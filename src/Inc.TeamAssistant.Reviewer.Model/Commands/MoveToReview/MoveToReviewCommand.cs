using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Reviewer.Model.Commands.MoveToReview;

public sealed record MoveToReviewCommand(MessageContext MessageContext, Guid DraftId)
    : IEndDialogCommand;