using Inc.TeamAssistant.Appraiser.Model.Commands.FinishStory;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.FinishStory.Services;

internal sealed class FinishStoryCommandCreator : ICommandCreator
{
    public string Command => CommandList.Finish;
    
    public Task<IEndDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);

        return Task.FromResult<IEndDialogCommand>(new FinishStoryCommand(
            messageContext,
            messageContext.TryParseId(Command)));
    }
}