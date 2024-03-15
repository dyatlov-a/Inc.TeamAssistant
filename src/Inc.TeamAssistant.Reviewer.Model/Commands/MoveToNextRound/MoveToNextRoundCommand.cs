using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Reviewer.Model.Commands.MoveToNextRound;

public sealed record MoveToNextRoundCommand(MessageContext MessageContext, Guid TaskId)
    : IEndDialogCommand;