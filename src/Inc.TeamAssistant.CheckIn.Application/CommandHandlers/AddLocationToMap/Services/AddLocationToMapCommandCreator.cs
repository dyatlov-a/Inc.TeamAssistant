using Inc.TeamAssistant.CheckIn.Model.Commands.AddLocationToMap;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.CheckIn.Application.CommandHandlers.AddLocationToMap.Services;

internal sealed class AddLocationToMapCommandCreator : ICommandCreator
{
    public string Command => CommandList.AddLocation;
    
    public Task<IDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);

        return Task.FromResult<IDialogCommand>(new AddLocationToMapCommand(messageContext));
    }
}