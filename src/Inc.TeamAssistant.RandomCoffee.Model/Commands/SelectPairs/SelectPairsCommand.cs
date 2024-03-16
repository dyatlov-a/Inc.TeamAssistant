using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.RandomCoffee.Model.Commands.SelectPairs;

public sealed record SelectPairsCommand(MessageContext MessageContext, Guid RandomCoffeeEntryId)
    : IDialogCommand;