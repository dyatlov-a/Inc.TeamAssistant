using System.Collections.Concurrent;
using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Primitives.Bots;
using Inc.TeamAssistant.Primitives.Extensions;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace Inc.TeamAssistant.Connector.Application.Telegram;

internal sealed class TelegramBotListeners : IBotListeners
{
    private readonly TelegramUpdateHandlerFactory _updateHandlerFactory;
    private readonly IBotReader _botReader;
    private readonly ReceiverOptions _receiverOptions;
    private readonly ILogger<TelegramBotListeners> _logger;
    private readonly ConcurrentDictionary<Guid, CancellationTokenSource> _listeners = new();
    private const int ShutdownValue = 0;
    private volatile int _isWorking = 1;

    public TelegramBotListeners(
        TelegramUpdateHandlerFactory updateHandlerFactory,
        IBotReader botReader,
        ReceiverOptions receiverOptions,
        ILogger<TelegramBotListeners> logger)
    {
        _updateHandlerFactory = updateHandlerFactory ?? throw new ArgumentNullException(nameof(updateHandlerFactory));
        _botReader = botReader ?? throw new ArgumentNullException(nameof(botReader));
        _receiverOptions = receiverOptions ?? throw new ArgumentNullException(nameof(receiverOptions));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Start(Guid botId, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        
        if (NotWorking())
            return;
        
        try
        {
            var listener = CancellationTokenSource.CreateLinkedTokenSource(token);
            var bot = await botId.Required((k, t) => _botReader.Find(k, DateTimeOffset.UtcNow, t), listener.Token);
            var client = new TelegramBotClient(bot.Token);
            var handler = _updateHandlerFactory.Create(botId);
            
            client.StartReceiving(handler, _receiverOptions, cancellationToken: listener.Token);

            _listeners.TryAdd(botId, listener);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bot {BotId} unhandled exception on start", botId);
        }
    }

    public async Task Restart(Guid botId, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        
        if (NotWorking())
            return;
        
        await Stop(botId, token);
        await Start(botId, token);
    }

    public async Task Stop(Guid botId, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        
        if (NotWorking())
            return;
        
        if (_listeners.Remove(botId, out var listener))
            using (listener)
                await listener.CancelAsync();
    }
    
    private bool NotWorking() => _isWorking == ShutdownValue;

    public async Task Shutdown(CancellationToken token)
    {
        Interlocked.Exchange(ref _isWorking, ShutdownValue);

        foreach (var listener in _listeners.Values)
        {
            token.ThrowIfCancellationRequested();
            
            using (listener)
                await listener.CancelAsync();
        }
    }
}