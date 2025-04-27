using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Reviewer.Model.Commands.AddComment;

public sealed record AddCommentCommand(
    MessageContext MessageContext,
    int ReplyToMessageId,
    string Comment)
    : IDialogCommand;