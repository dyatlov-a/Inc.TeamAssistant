using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Reviewer.Model.Commands.EditDraft;

public sealed record EditDraftCommand(MessageContext MessageContext, string Description)
    : IDialogCommand;