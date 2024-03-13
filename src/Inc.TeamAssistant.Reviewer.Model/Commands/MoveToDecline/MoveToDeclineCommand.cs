using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Reviewer.Model.Commands.MoveToDecline;

public sealed record MoveToDeclineCommand(MessageContext MessageContext, Guid TaskId)
    : IEndDialogCommand;