using Inc.TeamAssistant.Appraiser.Model.Commands.AcceptEstimate;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AcceptEstimate.Services;

internal sealed class AcceptEstimateCommandCreator : ICommandCreator
{
    public string Command => CommandList.AcceptEstimate;
    
    public Task<IRequest<CommandResult>> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        if (teamContext is null)
            throw new ArgumentNullException(nameof(teamContext));
        
        return Task.FromResult<IRequest<CommandResult>>(new AcceptEstimateCommand(
            messageContext,
            messageContext.TryParseId(Command)));
    }
}