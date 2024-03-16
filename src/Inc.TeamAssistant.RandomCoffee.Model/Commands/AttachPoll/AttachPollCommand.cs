using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.RandomCoffee.Model.Commands.AttachPoll;

public sealed record AttachPollCommand(MessageContext MainContext, Guid RandomCoffeeEntryId, string PollId)
    : IContinuationCommand;