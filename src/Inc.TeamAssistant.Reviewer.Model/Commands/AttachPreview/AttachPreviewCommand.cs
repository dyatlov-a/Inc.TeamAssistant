using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Reviewer.Model.Commands.AttachPreview;

public sealed record AttachPreviewCommand(
    MessageContext MainContext,
    Guid DraftId,
    int MessageId)
    : IContinuationCommand;