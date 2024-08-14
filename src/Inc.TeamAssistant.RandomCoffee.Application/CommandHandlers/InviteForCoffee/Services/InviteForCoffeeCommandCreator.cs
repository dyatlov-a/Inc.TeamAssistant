using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.RandomCoffee.Model.Commands.InviteForCoffee;

namespace Inc.TeamAssistant.RandomCoffee.Application.CommandHandlers.InviteForCoffee.Services;

internal sealed class InviteForCoffeeCommandCreator : ICommandCreator
{
    public string Command => CommandList.InviteForCoffee;
    
    public Task<IDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);

        return Task.FromResult<IDialogCommand>(new InviteForCoffeeCommand(messageContext, OnDemand: true));
    }
}