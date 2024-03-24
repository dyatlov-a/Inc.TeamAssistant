using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Reviewer.Model.Commands.MoveToDecline;

public sealed record MoveToDeclineCommand(MessageContext MessageContext, Guid TaskId)
    : IEndDialogCommand;