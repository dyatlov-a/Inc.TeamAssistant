using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Application.Services;
using Inc.TeamAssistant.Primitives.Commands;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Polling;

namespace Inc.TeamAssistant.Connector.Application.Telegram;

internal sealed class TelegramUpdateHandlerFactory
{
    private readonly ILogger<TelegramUpdateHandlerFactory> _logger;
    private readonly CommandFactory _commandFactory;
    private readonly ICommandExecutor _commandExecutor;
    private readonly TelegramMessageContextFactory _messageContextFactory;
    private readonly IBotReader _botReader;

    public TelegramUpdateHandlerFactory(
        ILogger<TelegramUpdateHandlerFactory> logger,
        CommandFactory commandFactory,
        ICommandExecutor commandExecutor,
        TelegramMessageContextFactory messageContextFactory,
        IBotReader botReader)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _commandFactory = commandFactory ?? throw new ArgumentNullException(nameof(commandFactory));
        _commandExecutor = commandExecutor ?? throw new ArgumentNullException(nameof(commandExecutor));
        _messageContextFactory = messageContextFactory ?? throw new ArgumentNullException(nameof(messageContextFactory));
        _botReader = botReader ?? throw new ArgumentNullException(nameof(botReader));
    }
    
    public IUpdateHandler Create(Guid botId)
    {
        return new TelegramBotMessageHandler(
            _logger,
            _commandFactory,
            _commandExecutor,
            _messageContextFactory,
            _botReader,
            botId);
    }
}