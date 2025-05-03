using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.RandomCoffee.Model.Commands.AttachPoll;

public sealed record AttachPollCommand(
    MessageContext MainContext,
    Guid RandomCoffeeEntryId,
    string PollId,
    int MessageId)
    : IContinuationCommand;