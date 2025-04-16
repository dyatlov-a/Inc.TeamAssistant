using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.RandomCoffee.Model.Commands.AddPollAnswer;

namespace Inc.TeamAssistant.RandomCoffee.Application.CommandHandlers.AddPollAnswer.Services;

internal sealed class AddPollAnswerCommandCreator : ICommandCreator
{
    public string Command => CommandList.AddPollAnswer;
    
    public Task<IDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);

        var parameters = messageContext.Text
            .Replace(CommandList.AddPollAnswer, string.Empty)
            .Split(GlobalResources.Settings.OptionParameterName);
        var pollId = parameters[0];
        var options = parameters.Skip(1).ToArray();
        
        return Task.FromResult<IDialogCommand>(new AddPollAnswerCommand(messageContext, pollId, options));
    }
}