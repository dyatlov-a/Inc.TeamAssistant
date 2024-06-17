using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Primitives.Commands;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Polling;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class UpdateHandlerFactory
{
    private readonly ILogger<UpdateHandlerFactory> _logger;
    private readonly CommandFactory _commandFactory;
    private readonly ICommandExecutor _commandExecutor;
    private readonly MessageContextBuilder _messageContextBuilder;
    private readonly IBotReader _botReader;

    public UpdateHandlerFactory(
        ILogger<UpdateHandlerFactory> logger,
        CommandFactory commandFactory,
        ICommandExecutor commandExecutor,
        MessageContextBuilder messageContextBuilder,
        IBotReader botReader)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _commandFactory = commandFactory ?? throw new ArgumentNullException(nameof(commandFactory));
        _commandExecutor = commandExecutor ?? throw new ArgumentNullException(nameof(commandExecutor));
        _messageContextBuilder =
            messageContextBuilder ?? throw new ArgumentNullException(nameof(messageContextBuilder));
        _botReader = botReader ?? throw new ArgumentNullException(nameof(botReader));
    }
    
    public IUpdateHandler Create(Guid botId)
    {
        return new TelegramBotMessageHandler(
            _logger,
            _commandFactory,
            _commandExecutor,
            _messageContextBuilder,
            _botReader,
            botId);
    }
}