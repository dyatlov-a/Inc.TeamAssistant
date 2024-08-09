using Inc.TeamAssistant.Appraiser.Model.Commands.FinishEstimate;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.FinishEstimate.Services;

internal sealed class FinishEstimateCommandCreator : ICommandCreator
{
    public string Command => CommandList.Finish;
    
    public Task<IEndDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);

        return Task.FromResult<IEndDialogCommand>(new FinishEstimateCommand(
            messageContext,
            messageContext.TryParseId(Command)));
    }
}