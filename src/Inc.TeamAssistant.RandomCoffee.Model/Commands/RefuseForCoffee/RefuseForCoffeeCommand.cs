using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.RandomCoffee.Model.Commands.RefuseForCoffee;

public sealed record RefuseForCoffeeCommand(MessageContext MessageContext)
    : IDialogCommand;