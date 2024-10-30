using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.RandomCoffee.Model.Commands.RefuseForCoffee;

namespace Inc.TeamAssistant.RandomCoffee.Application.CommandHandlers.RefuseForCoffee.Services;

internal sealed class RefuseForCoffeeCommandCreator : ICommandCreator
{
    public string Command => CommandList.RefuseForCoffee;
    
    public Task<IDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);

        return Task.FromResult<IDialogCommand>(new RefuseForCoffeeCommand(messageContext));
    }
}