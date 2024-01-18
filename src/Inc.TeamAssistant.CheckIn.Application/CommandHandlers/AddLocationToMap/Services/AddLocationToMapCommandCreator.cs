using Inc.TeamAssistant.CheckIn.Model.Commands.AddLocationToMap;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.CheckIn.Application.CommandHandlers.AddLocationToMap.Services;

internal sealed class AddLocationToMapCommandCreator : ICommandCreator
{
    public string Command => CommandList.AddLocation;
    
    public Task<IRequest<CommandResult>> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        if (teamContext is null)
            throw new ArgumentNullException(nameof(teamContext));

        return Task.FromResult<IRequest<CommandResult>>(new AddLocationToMapCommand(messageContext));
    }
}