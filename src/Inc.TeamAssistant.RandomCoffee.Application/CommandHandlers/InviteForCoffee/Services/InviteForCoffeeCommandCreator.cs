using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.RandomCoffee.Model.Commands.InviteForCoffee;

namespace Inc.TeamAssistant.RandomCoffee.Application.CommandHandlers.InviteForCoffee.Services;

internal sealed class InviteForCoffeeCommandCreator : ICommandCreator
{
    private readonly string _command = CommandList.InviteForCoffee;
    
    public IDialogCommand? TryCreate(
        string command,
        bool singleLineMode,
        MessageContext messageContext,
        CurrentTeamContext teamContext)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);
        
        if (singleLineMode || !command.StartsWith(_command, StringComparison.InvariantCultureIgnoreCase))
            return null;

        return new InviteForCoffeeCommand(messageContext, OnDemand: true);
    }
}