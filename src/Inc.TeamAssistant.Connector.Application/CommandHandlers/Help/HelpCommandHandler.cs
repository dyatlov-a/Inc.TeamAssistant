using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Model.Commands.Help;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Primitives.Notifications;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.Help;

internal sealed class HelpCommandHandler : IRequestHandler<HelpCommand, CommandResult>
{
    private readonly IBotReader _reader;
    private readonly IMessageBuilder _messageBuilder;

    public HelpCommandHandler(IBotReader reader, IMessageBuilder messageBuilder)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

    public async Task<CommandResult> Handle(HelpCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var bot = await command.MessageContext.Bot.Id.Required(
            (id, t) => _reader.Find(id, DateTimeOffset.UtcNow, t),
            token);
        var notification = NotificationMessage
            .Create(
                command.MessageContext.ChatMessage.ChatId,
                await _messageBuilder.Build(
                    Messages.Connector_HelpText,
                    command.MessageContext.LanguageId,
                    CommandList.Cancel))
            .SetButtonsInRow(1);
        var commands = bot.Commands.Where(c =>
            c.HelpMessageId is not null &&
            !CommandList.Help.Equals(c.Value, StringComparison.InvariantCultureIgnoreCase));

        foreach (var cmd in commands)
        {
            var text = await _messageBuilder.Build(cmd.HelpMessageId!, command.MessageContext.LanguageId);

            notification.WithButton(new Button(text, cmd.Value));
        }

        var deleteHelp = NotificationMessage.Delete(command.MessageContext.ChatMessage);
        
        return CommandResult.Build(notification, deleteHelp);
    }
}