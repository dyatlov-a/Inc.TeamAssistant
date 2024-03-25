using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.RandomCoffee.Model.Commands.SelectPairs;

public sealed record SelectPairsCommand(MessageContext MessageContext, Guid RandomCoffeeEntryId)
    : IDialogCommand;