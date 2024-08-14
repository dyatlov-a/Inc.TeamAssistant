using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Reviewer.Model.Commands.CancelDraft;

public sealed record CancelDraftCommand(MessageContext MessageContext, Guid DraftId)
    : IDialogCommand;