using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Model.Commands.Help;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Primitives.Notifications;
using MediatR;
using ArgumentNullException = System.ArgumentNullException;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.Help;

internal sealed class HelpCommandHandler : IRequestHandler<HelpCommand, CommandResult>
{
    private readonly IBotReader _botReader;
    private readonly IMessageBuilder _messageBuilder;

    public HelpCommandHandler(IBotReader botReader, IMessageBuilder messageBuilder)
    {
        _botReader = botReader ?? throw new ArgumentNullException(nameof(botReader));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

    public async Task<CommandResult> Handle(HelpCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var bot = await _botReader.Find(command.MessageContext.Bot.Id, DateTimeOffset.UtcNow, token);
        if (bot is null)
            throw new TeamAssistantUserException(Messages.Connector_BotNotFound, command.MessageContext.Bot.Id);

        var notificationText = await _messageBuilder.Build(
            Messages.Connector_HelpText,
            command.MessageContext.LanguageId,
            CommandList.Cancel);
        var notification = NotificationMessage
            .Create(command.MessageContext.ChatMessage.ChatId, notificationText)
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