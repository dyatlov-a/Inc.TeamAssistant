using Inc.TeamAssistant.Connector.Model.Commands.JoinToTeam;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.JoinToTeam.Services;

internal sealed class JoinToTeamCommandCreator : ICommandCreator
{
    public string Command => "/start";
    
    public Task<IRequest<CommandResult>> Create(
        MessageContext messageContext,
        Guid? selectedTeamId,
        CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        
        var botToken = messageContext.Text
            .Replace(Command, string.Empty, StringComparison.InvariantCultureIgnoreCase)
            .Trim();

        if (!Guid.TryParse(botToken, out var value))
            throw new ApplicationException("Can not starting the bot. Please move by link for start.");
        
        return Task.FromResult<IRequest<CommandResult>>(new JoinToTeamCommand(messageContext, value));
    }
}