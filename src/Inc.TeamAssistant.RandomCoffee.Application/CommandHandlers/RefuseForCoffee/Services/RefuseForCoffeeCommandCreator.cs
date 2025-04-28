using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.RandomCoffee.Model.Commands.RefuseForCoffee;

namespace Inc.TeamAssistant.RandomCoffee.Application.CommandHandlers.RefuseForCoffee.Services;

internal sealed class RefuseForCoffeeCommandCreator : ICommandCreator
{
    private readonly string _command = CommandList.RefuseForCoffee;
    
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

        return new RefuseForCoffeeCommand(messageContext);
    }
}