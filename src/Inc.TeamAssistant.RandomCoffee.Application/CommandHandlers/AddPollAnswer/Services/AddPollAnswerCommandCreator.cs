using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.RandomCoffee.Model.Commands.AddPollAnswer;

namespace Inc.TeamAssistant.RandomCoffee.Application.CommandHandlers.AddPollAnswer.Services;

internal sealed class AddPollAnswerCommandCreator : ICommandCreator
{
    public string Command => CommandList.AddPollAnswer;
    public bool SupportSingleLineMode => false;
    
    public Task<IEndDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);

        var parameters = messageContext.Text.Replace(CommandList.AddPollAnswer, string.Empty).Split("&option=");
        var pollId = parameters[0];
        var options = parameters.Skip(1).ToArray();
        
        return Task.FromResult<IEndDialogCommand>(new AddPollAnswerCommand(messageContext, pollId, options));
    }
}