using Inc.TeamAssistant.Connector.Model.Commands.JoinToTeam;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.JoinToTeam.Services;

internal sealed class JoinToTeamCommandCreator : ICommandCreator
{
    private readonly string _command = "/start";
    
    public int Priority => 4;
    
    public Task<IRequest<CommandResult>?> Create(MessageContext messageContext)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        
        if (messageContext.Cmd.StartsWith(_command, StringComparison.InvariantCultureIgnoreCase))
        {
            var token = messageContext.Text
                .Replace(_command, string.Empty, StringComparison.InvariantCultureIgnoreCase)
                .Trim();

            if (Guid.TryParse(token, out var value))
                return Task.FromResult<IRequest<CommandResult>?>(new JoinToTeamCommand(messageContext, value));
        }

        return Task.FromResult<IRequest<CommandResult>?>(null);
    }
}