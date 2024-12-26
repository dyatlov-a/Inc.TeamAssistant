using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Application.Extensions;
using Inc.TeamAssistant.Primitives.Commands;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class TelegramBotMessageHandler : IUpdateHandler
{
    private readonly ILogger _logger;
    private readonly CommandFactory _commandFactory;
    private readonly ICommandExecutor _commandExecutor;
    private readonly MessageContextBuilder _messageContextBuilder;
    private readonly IBotReader _botReader;
    private readonly Guid _botId;

    public TelegramBotMessageHandler(
        ILogger logger,
        CommandFactory commandFactory,
        ICommandExecutor commandExecutor,
        MessageContextBuilder messageContextBuilder,
        IBotReader botReader,
        Guid botId)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _commandFactory = commandFactory ?? throw new ArgumentNullException(nameof(commandFactory));
        _commandExecutor = commandExecutor ?? throw new ArgumentNullException(nameof(commandExecutor));
        _messageContextBuilder =
            messageContextBuilder ?? throw new ArgumentNullException(nameof(messageContextBuilder));
        _botReader = botReader ?? throw new ArgumentNullException(nameof(botReader));
        _botId = botId;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(botClient);
        ArgumentNullException.ThrowIfNull(update);

        using var logScope = _logger.BeginScope("Bot {BotId} User {User}", _botId, update.GetUserName());
        
        try
        {
            var bot = await _botReader.Find(_botId, DateTimeOffset.UtcNow, token);
            if (bot is not null)
            {
                var messageContext = await _messageContextBuilder.Build(bot, update, token);
                if (messageContext is not null)
                {
                    var command = await _commandFactory.TryCreate(bot, messageContext, token);
                    if (command is not null)
                        await _commandExecutor.Execute(command, token);
                }
            }

            if (update.CallbackQuery is not null)
                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, cancellationToken: token);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Unhandled exception on handle message");
        }
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(botClient);

        _logger.LogWarning(exception, "Bot {BotId} listened message with error", _botId);
        
        return Task.CompletedTask;
    }
}