using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.RandomCoffee.Model.Commands.RefuseForCoffee;

namespace Inc.TeamAssistant.RandomCoffee.Application.CommandHandlers.RefuseForCoffee.Services;

internal sealed class RefuseForCoffeeCommandCreator : ICommandCreator
{
    private readonly string _command = CommandList.RefuseForCoffee;
    
    public Task<IDialogCommand?> TryCreate(
        string command,
        bool singleLineMode,
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);
        
        if (singleLineMode || !command.StartsWith(_command, StringComparison.InvariantCultureIgnoreCase))
            return Task.FromResult<IDialogCommand?>(null);

        return Task.FromResult<IDialogCommand?>(new RefuseForCoffeeCommand(messageContext));
    }
}