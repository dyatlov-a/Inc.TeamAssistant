using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Reviewer.Model.Commands.MoveToNextRound;

public sealed record MoveToNextRoundCommand(MessageContext MessageContext, Guid TaskId)
    : IDialogCommand;