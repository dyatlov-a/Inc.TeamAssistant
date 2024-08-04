using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Reviewer.Model.Commands.NeedReview;

public sealed record NeedReviewCommand(
    MessageContext MessageContext,
    Guid TeamId,
    string Strategy,
    string Description)
    : IEndDialogCommand
{
    public bool SaveEndOfDialog => true;
}