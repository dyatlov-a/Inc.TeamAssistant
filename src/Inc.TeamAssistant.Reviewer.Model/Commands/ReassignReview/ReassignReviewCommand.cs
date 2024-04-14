using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Reviewer.Model.Commands.ReassignReview;

public sealed record ReassignReviewCommand(MessageContext MessageContext, Guid TaskId)
    : IEndDialogCommand;