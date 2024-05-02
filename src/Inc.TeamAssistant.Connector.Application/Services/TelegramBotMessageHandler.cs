using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Primitives;
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
    private readonly IBotRepository _botRepository;

    public TelegramBotMessageHandler(
        ILogger<TelegramBotMessageHandler> logger,
        CommandFactory commandFactory,
        ICommandExecutor commandExecutor,
        MessageContextBuilder messageContextBuilder,
        IBotRepository botRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _commandFactory = commandFactory ?? throw new ArgumentNullException(nameof(commandFactory));
        _commandExecutor = commandExecutor ?? throw new ArgumentNullException(nameof(commandExecutor));
        _messageContextBuilder = messageContextBuilder ?? throw new ArgumentNullException(nameof(messageContextBuilder));
        _botRepository = botRepository ?? throw new ArgumentNullException(nameof(botRepository));
    }

    public async Task Handle(Update update, BotContext botContext, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(update);
        ArgumentNullException.ThrowIfNull(botContext);

        try
        {
            var bot = await _botRepository.Find(botContext.Id, token);
            if (bot is null)
                return;
            
            var messageContext = await _messageContextBuilder.Build(bot, botContext, update, token);
            if (messageContext is null)
                return;
        
            var command = await _commandFactory.TryCreate(bot, messageContext, token);
            if (command is not null)
                await _commandExecutor.Execute(command, token);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Bot {BotId} unhandled exception on handle message", botContext.Id);
        }
    }

    public Task OnError(Exception exception, BotContext botContext, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(botContext);
        
        _logger.LogError(exception, "Bot {BotId} listened message with error", botContext.Id);

        return Task.CompletedTask;
    }
}