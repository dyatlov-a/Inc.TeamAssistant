using Inc.TeamAssistant.CheckIn.Model.Commands.AddLocationToMap;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.CheckIn.Application.CommandHandlers.AddLocationToMap.Services;

internal sealed class AddLocationToMapCommandCreator : ICommandCreator
{
    private readonly string _command = "/location";
    
    public int Priority => 1;
    
    public Task<IRequest<CommandResult>?> Create(MessageContext messageContext, CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));

        if (messageContext.Cmd.Equals(_command, StringComparison.InvariantCultureIgnoreCase))
            return Task.FromResult<IRequest<CommandResult>?>(new AddLocationToMapCommand(messageContext));

        return Task.FromResult<IRequest<CommandResult>?>(null);
    }
}