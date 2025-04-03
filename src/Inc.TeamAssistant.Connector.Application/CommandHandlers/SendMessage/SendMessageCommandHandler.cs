using Inc.TeamAssistant.Connector.Model.Commands.SendMessage;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Primitives.Notifications;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.SendMessage;

internal sealed class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, CommandResult>
{
    private readonly IMessageBuilder _messageBuilder;

    public SendMessageCommandHandler(IMessageBuilder messageBuilder)
    {
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

    public Task<CommandResult> Handle(SendMessageCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var message = _messageBuilder.Build(
            command.MessageId,
            command.MessageContext.LanguageId,
            command.Values);
        var notification = NotificationMessage.Create(
            command.MessageContext.ChatMessage.ChatId,
            message);
        
        return Task.FromResult(CommandResult.Build(notification));
    }
}