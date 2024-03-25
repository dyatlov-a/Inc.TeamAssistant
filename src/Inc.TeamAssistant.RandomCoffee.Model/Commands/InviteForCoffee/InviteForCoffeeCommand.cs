using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.RandomCoffee.Model.Commands.InviteForCoffee;

public sealed record InviteForCoffeeCommand(MessageContext MessageContext, bool OnDemand)
    : IEndDialogCommand;