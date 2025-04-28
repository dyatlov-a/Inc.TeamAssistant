using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToDecline;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToDecline.Services;

internal sealed class MoveToDeclineCommandCreator : ICommandCreator
{
    private readonly string _command = CommandList.Decline;
    
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

        return new MoveToDeclineCommand(messageContext, messageContext.TryParseId(_command));
    }
}