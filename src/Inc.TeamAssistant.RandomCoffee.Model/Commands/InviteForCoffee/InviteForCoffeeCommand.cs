using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.RandomCoffee.Model.Commands.InviteForCoffee;

public sealed record InviteForCoffeeCommand(MessageContext MessageContext, bool OnDemand)
    : IEndDialogCommand;