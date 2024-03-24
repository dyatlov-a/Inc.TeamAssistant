using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Reviewer.Model.Commands.MoveToInProgress;

public sealed record MoveToInProgressCommand(MessageContext MessageContext, Guid TaskId)
    : IEndDialogCommand;