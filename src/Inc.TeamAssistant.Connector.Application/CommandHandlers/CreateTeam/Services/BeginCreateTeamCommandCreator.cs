using Inc.TeamAssistant.Connector.Model.Commands.MarkMessageForDelete;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands.Begin;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.CreateTeam.Services;

internal sealed class BeginCreateTeamCommandCreator : ICommandCreator
{
    private readonly IMessageBuilder _messageBuilder;
    private readonly string _command = "/new_team";
    
    public int Priority => 4;

    public BeginCreateTeamCommandCreator(IMessageBuilder messageBuilder)
    {
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }
    
    public async Task<IRequest<CommandResult>?> Create(MessageContext messageContext, CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        
        if (messageContext.Cmd.Equals(_command, StringComparison.InvariantCultureIgnoreCase))
        {
            var notification = NotificationMessage.Create(
                messageContext.ChatId,
                await _messageBuilder.Build(Messages.Connector_EnterTeamName, messageContext.LanguageId));
            if (messageContext.Shared)
                notification.AddHandler((c, mId) => new MarkMessageForDeleteCommand(c, mId));
            
            return new BeginCommand(messageContext, BotCommandStage.EnterText, _command, notification);
        }

        return null;
    }
}