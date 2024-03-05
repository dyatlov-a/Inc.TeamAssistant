using System.Text;
using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Model.Commands.Help;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Exceptions;
using MediatR;
using ArgumentNullException = System.ArgumentNullException;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.Help;

internal sealed class HelpCommandHandler : IRequestHandler<HelpCommand, CommandResult>
{
    private readonly IBotRepository _botRepository;
    private readonly IMessageBuilder _messageBuilder;

    public HelpCommandHandler(IBotRepository botRepository, IMessageBuilder messageBuilder)
    {
        _botRepository = botRepository ?? throw new ArgumentNullException(nameof(botRepository));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

    public async Task<CommandResult> Handle(HelpCommand command, CancellationToken token)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var bot = await _botRepository.Find(command.MessageContext.BotId, token);
        if (bot is null)
            throw new TeamAssistantUserException(Messages.Connector_BotNotFound, command.MessageContext.BotId);
        
        var builder = new StringBuilder();
        foreach (var cmd in bot.Commands.Where(c => c.HelpMessageId is not null))
            builder.AppendLine(await _messageBuilder.Build(
                cmd.HelpMessageId!,
                command.MessageContext.LanguageId,
                cmd.Value));

        return CommandResult.Build(NotificationMessage.Create(command.MessageContext.ChatId, builder.ToString()));
    }
}