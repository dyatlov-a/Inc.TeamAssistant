using Inc.TeamAssistant.Appraiser.Model.Commands.AcceptEstimate;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AcceptEstimate.Services;

internal sealed class AcceptEstimateCommandCreator : ICommandCreator
{
    public string Command => "/accept?storyId=";
    
    public Task<IRequest<CommandResult>> Create(
        MessageContext messageContext,
        Guid? selectedTeamId,
        CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));

        var storyId = Guid.Parse(messageContext.Text.Replace(Command, string.Empty));
        return Task.FromResult<IRequest<CommandResult>>(new AcceptEstimateCommand(messageContext, storyId));
    }
}