using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Application.Services;
using Inc.TeamAssistant.Primitives.Commands;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Inc.TeamAssistant.Connector.Application.Telegram;

internal sealed class TelegramBotMessageHandler : IUpdateHandler
{
    private readonly ILogger _logger;
    private readonly CommandFactory _commandFactory;
    private readonly ICommandExecutor _commandExecutor;
    private readonly TelegramMessageContextFactory _messageContextFactory;
    private readonly IBotReader _botReader;
    private readonly Guid _botId;

    public TelegramBotMessageHandler(
        ILogger logger,
        CommandFactory commandFactory,
        ICommandExecutor commandExecutor,
        TelegramMessageContextFactory messageContextFactory,
        IBotReader botReader,
        Guid botId)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _commandFactory = commandFactory ?? throw new ArgumentNullException(nameof(commandFactory));
        _commandExecutor = commandExecutor ?? throw new ArgumentNullException(nameof(commandExecutor));
        _messageContextFactory = messageContextFactory ?? throw new ArgumentNullException(nameof(messageContextFactory));
        _botReader = botReader ?? throw new ArgumentNullException(nameof(botReader));
        _botId = botId;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(botClient);
        ArgumentNullException.ThrowIfNull(update);

        using var logScope = _logger.BeginScope("Bot {BotId} User {User}", _botId, GetUserName(update));
        
        try
        {
            var bot = await _botReader.Find(_botId, DateTimeOffset.UtcNow, token);
            if (bot is not null)
            {
                var messageContext = await _messageContextFactory.Create(bot, update, token);
                if (messageContext is not null)
                {
                    var command = await _commandFactory.TryCreate(bot, messageContext, token);
                    if (command is not null)
                        await _commandExecutor.Execute(command, token);
                }
                
                if (update.CallbackQuery is not null)
                    await botClient.AnswerCallbackQueryAsync(
                        update.CallbackQuery.Id,
                        "Done",
                        false,
                        cancellationToken: token);
            }
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
    
    private static string GetUserName(Update update)
    {
        ArgumentNullException.ThrowIfNull(update);
        
        return update.Type switch
        {
            UpdateType.Message or UpdateType.EditedMessage => ToLogEntry(update.Message?.From),
            UpdateType.CallbackQuery => ToLogEntry(update.CallbackQuery?.From),
            UpdateType.PollAnswer => ToLogEntry(update.PollAnswer?.User),
            _ => string.Empty
        };
    }
    
    private static string ToLogEntry(User? user)
    {
        var result = user is null
            ? string.Empty
            : $"(Id: {user.Id}, FirstName: {user.FirstName}, Username: {user.Username})";

        return result;
    }
}