using System.Collections.Concurrent;
using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Primitives.Bots;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class BotListeners : IBotListeners
{
    private readonly UpdateHandlerFactory _updateHandlerFactory;
    private readonly IBotReader _botReader;
    private readonly ILogger<BotListeners> _logger;
    
    private static readonly ReceiverOptions ReceiverOptions = new()
    {
        AllowedUpdates = [UpdateType.Message, UpdateType.CallbackQuery, UpdateType.PollAnswer, UpdateType.EditedMessage]
    };
    private readonly ConcurrentDictionary<Guid, CancellationTokenSource> _listeners = new();
    private volatile int _isWorking = 1;

    public BotListeners(UpdateHandlerFactory updateHandlerFactory, IBotReader botReader, ILogger<BotListeners> logger)
    {
        _updateHandlerFactory = updateHandlerFactory ?? throw new ArgumentNullException(nameof(updateHandlerFactory));
        _botReader = botReader ?? throw new ArgumentNullException(nameof(botReader));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Start(Guid botId)
    {
        if (NotWorking())
            return;
        
        try
        {
            var listener = new CancellationTokenSource();
            var bot = await _botReader.Find(botId, DateTimeOffset.UtcNow, listener.Token);
            var client = new TelegramBotClient(bot!.Token);
            var handler = _updateHandlerFactory.Create(botId);
            
            client.StartReceiving(handler, ReceiverOptions, cancellationToken: listener.Token);
            
            _listeners.TryAdd(botId, listener);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bot {BotId} unhandled exception on start", botId);
        }
    }

    public async Task Restart(Guid botId)
    {
        if (NotWorking())
            return;
        
        await Stop(botId);
        await Start(botId);
    }

    public async Task Stop(Guid botId)
    {
        if (NotWorking())
            return;
        
        if (_listeners.Remove(botId, out var listener))
            using (listener)
                await listener.CancelAsync();
    }
    
    private bool NotWorking() => _isWorking == 0;

    public async Task Shutdown(CancellationToken token)
    {
        Interlocked.Exchange(ref _isWorking, 0);

        foreach (var listener in _listeners.Values)
        {
            token.ThrowIfCancellationRequested();
            
            using (listener)
                await listener.CancelAsync();
        }
    }
}