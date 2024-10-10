using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Reviewer.Model.Commands.MoveToAccept;

public sealed record MoveToAcceptCommand(MessageContext MessageContext, Guid TaskId, bool AcceptedWithComments)
    : IDialogCommand;