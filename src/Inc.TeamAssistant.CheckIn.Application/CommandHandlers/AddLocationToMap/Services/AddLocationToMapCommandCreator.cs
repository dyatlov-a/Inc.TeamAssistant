using Inc.TeamAssistant.CheckIn.Model.Commands.AddLocationToMap;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.CheckIn.Application.CommandHandlers.AddLocationToMap.Services;

internal sealed class AddLocationToMapCommandCreator : ICommandCreator
{
    public string Command => "/location";
    
    public Task<IRequest<CommandResult>> Create(
        MessageContext messageContext,
        Guid? selectedTeamId,
        CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));

        return Task.FromResult<IRequest<CommandResult>>(new AddLocationToMapCommand(messageContext));
    }
}