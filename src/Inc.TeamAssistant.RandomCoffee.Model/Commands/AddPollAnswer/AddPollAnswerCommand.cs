using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.RandomCoffee.Model.Commands.AddPollAnswer;

public sealed record AddPollAnswerCommand(
    MessageContext MessageContext,
    string PollId,
    IReadOnlyCollection<string> Options)
    : IEndDialogCommand;