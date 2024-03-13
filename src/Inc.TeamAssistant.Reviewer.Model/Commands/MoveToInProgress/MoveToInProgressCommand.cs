using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Reviewer.Model.Commands.MoveToInProgress;

public sealed record MoveToInProgressCommand(MessageContext MessageContext, Guid TaskId)
    : IEndDialogCommand;