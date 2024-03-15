using Inc.TeamAssistant.Appraiser.Model.Commands.AcceptEstimate;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AcceptEstimate.Services;

internal sealed class AcceptEstimateCommandCreator : ICommandCreator
{
    public string Command => CommandList.AcceptEstimate;
    
    public Task<IEndDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        if (teamContext is null)
            throw new ArgumentNullException(nameof(teamContext));
        
        return Task.FromResult<IEndDialogCommand>(new AcceptEstimateCommand(
            messageContext,
            messageContext.TryParseId(Command)));
    }
}