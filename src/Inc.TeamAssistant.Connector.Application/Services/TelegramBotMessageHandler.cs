using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Primitives.Commands;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class TelegramBotMessageHandler
{
    private readonly ILogger<TelegramBotMessageHandler> _logger;
    private readonly CommandFactory _commandFactory;
    private readonly ICommandExecutor _commandExecutor;
    private readonly MessageContextBuilder _messageContextBuilder;

    public TelegramBotMessageHandler(
        ILogger<TelegramBotMessageHandler> logger,
        CommandFactory commandFactory,
        ICommandExecutor commandExecutor,
        MessageContextBuilder messageContextBuilder)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _commandFactory = commandFactory ?? throw new ArgumentNullException(nameof(commandFactory));
        _commandExecutor = commandExecutor ?? throw new ArgumentNullException(nameof(commandExecutor));
        _messageContextBuilder = messageContextBuilder ?? throw new ArgumentNullException(nameof(messageContextBuilder));
    }

    public async Task Handle(Update update, Bot bot, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(update);
        ArgumentNullException.ThrowIfNull(bot);

        try
        {
            var messageContext = await _messageContextBuilder.Build(bot, update, token);
            if (messageContext is null)
                return;
        
            var command = await _commandFactory.TryCreate(bot, messageContext, token);
            if (command is not null)
                await _commandExecutor.Execute(command, token);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Bot {BotId} unhandled exception on handle message", bot.Id);
        }
    }

    public Task OnError(Exception exception, Bot bot, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(bot);

        _logger.LogError(exception, "Bot {BotId} listened message with error", bot.Id);

        return Task.CompletedTask;
    }
}