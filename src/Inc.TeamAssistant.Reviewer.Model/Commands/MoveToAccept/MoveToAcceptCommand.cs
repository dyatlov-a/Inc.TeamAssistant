using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Reviewer.Model.Commands.MoveToAccept;

public sealed record MoveToAcceptCommand(MessageContext MessageContext, Guid TaskId)
    : IEndDialogCommand;