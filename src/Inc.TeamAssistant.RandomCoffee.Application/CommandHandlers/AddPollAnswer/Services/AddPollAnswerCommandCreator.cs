using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.RandomCoffee.Model.Commands.AddPollAnswer;

namespace Inc.TeamAssistant.RandomCoffee.Application.CommandHandlers.AddPollAnswer.Services;

internal sealed class AddPollAnswerCommandCreator : ICommandCreator
{
    public string Command => CommandList.AddPollAnswer;
    
    public Task<IEndDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        if (teamContext is null)
            throw new ArgumentNullException(nameof(teamContext));

        var parameters = messageContext.Text.Replace(CommandList.AddPollAnswer, string.Empty).Split("&option=");
        var pollId = parameters[0];
        var options = parameters.Skip(1).ToArray();
        
        return Task.FromResult<IEndDialogCommand>(new AddPollAnswerCommand(messageContext, pollId, options));
    }
}