using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.RandomCoffee.Model.Commands.InviteForCoffee;

namespace Inc.TeamAssistant.RandomCoffee.Application.CommandHandlers.InviteForCoffee.Services;

internal sealed class InviteForCoffeeCommandCreator : ICommandCreator
{
    public string Command => CommandList.InviteForCoffee;
    
    public Task<IEndDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        if (teamContext is null)
            throw new ArgumentNullException(nameof(teamContext));

        return Task.FromResult<IEndDialogCommand>(new InviteForCoffeeCommand(messageContext, OnDemand: true));
    }
}