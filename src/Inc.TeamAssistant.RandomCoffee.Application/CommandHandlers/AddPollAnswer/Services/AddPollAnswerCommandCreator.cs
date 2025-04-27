using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.RandomCoffee.Model.Commands.AddPollAnswer;

namespace Inc.TeamAssistant.RandomCoffee.Application.CommandHandlers.AddPollAnswer.Services;

internal sealed class AddPollAnswerCommandCreator : ICommandCreator
{
    private readonly string _command = CommandList.AddPollAnswer;
    
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

        var parameters = messageContext.Text
            .Replace(CommandList.AddPollAnswer, string.Empty)
            .Split(GlobalResources.Settings.OptionParameterName);
        var pollId = parameters[0];
        var options = parameters.Skip(1).ToArray();
        
        return new AddPollAnswerCommand(messageContext, pollId, options);
    }
}