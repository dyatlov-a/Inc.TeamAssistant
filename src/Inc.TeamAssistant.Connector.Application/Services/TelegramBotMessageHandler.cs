using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Exceptions;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class TelegramBotMessageHandler
{
    private readonly ILogger<TelegramBotMessageHandler> _logger;
    private readonly IBotRepository _botRepository;
    private readonly CommandFactory _commandFactory;
    private readonly ICommandExecutor _commandExecutor;
    private readonly MessageContextBuilder _messageContextBuilder;

    public TelegramBotMessageHandler(
        ILogger<TelegramBotMessageHandler> logger,
        IBotRepository botRepository,
        CommandFactory commandFactory,
        ICommandExecutor commandExecutor,
        MessageContextBuilder messageContextBuilder)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _botRepository = botRepository ?? throw new ArgumentNullException(nameof(botRepository));
        _commandFactory = commandFactory ?? throw new ArgumentNullException(nameof(commandFactory));
        _commandExecutor = commandExecutor ?? throw new ArgumentNullException(nameof(commandExecutor));
        _messageContextBuilder =
            messageContextBuilder ?? throw new ArgumentNullException(nameof(messageContextBuilder));
    }

    public async Task Handle(ITelegramBotClient client, Update update, Guid botId, CancellationToken token)
    {
        if (client is null)
            throw new ArgumentNullException(nameof(client));
        if (update is null)
            throw new ArgumentNullException(nameof(update));
        
        try
        {
            var bot = await _botRepository.Find(botId, token);
            if (bot is null)
                throw new TeamAssistantUserException(Messages.Connector_BotNotFound, botId);

            var messageContext = await _messageContextBuilder.Build(bot, update, token);
            if (messageContext is null)
                return;
        
            var command = await _commandFactory.TryCreate(bot, messageContext, token);
            if (command is not null)
                await _commandExecutor.Execute(command, token);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Unhandled exception");
        }
    }

    public Task OnError(ITelegramBotClient client, Exception exception, string botName, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(botName))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(botName));

        _logger.LogError(exception, "Bot {BotName} listened message with error", botName);

        return Task.CompletedTask;
    }
}