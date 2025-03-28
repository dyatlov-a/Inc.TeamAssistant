using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.RandomCoffee.Model.Commands.AddPollAnswer;

public sealed record AddPollAnswerCommand(
    MessageContext MessageContext,
    string PollId,
    IReadOnlyCollection<string> Options)
    : IDialogCommand
{
    private const string Yes = "0";

    public bool IsAttend => Options.Contains(Yes);
}