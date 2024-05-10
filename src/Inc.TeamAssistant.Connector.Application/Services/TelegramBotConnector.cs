using System.Collections.Concurrent;
using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Primitives.Bots;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class TelegramBotConnector : IHostedService, IBotListener
{
    private readonly TelegramBotMessageHandler _handler;
    private readonly IBotReader _botReader;
    private readonly BotConstructor _botConstructor;
    private readonly ILogger<TelegramBotConnector> _logger;
    
    private readonly ConcurrentDictionary<Guid, CancellationTokenSource> _listeners = new();
    private bool _isWorking;
    
    private static readonly ReceiverOptions ReceiverOptions = new()
    {
        AllowedUpdates = [UpdateType.Message, UpdateType.CallbackQuery, UpdateType.PollAnswer]
    };
    
    public TelegramBotConnector(
        TelegramBotMessageHandler handler,
        IBotReader botReader,
        BotConstructor botConstructor,
        ILogger<TelegramBotConnector> logger)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        _botReader = botReader ?? throw new ArgumentNullException(nameof(botReader));
        _botConstructor = botConstructor ?? throw new ArgumentNullException(nameof(botConstructor));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task StartAsync(CancellationToken token)
    {
        _isWorking = true;
        var botIds = await _botReader.GetBotIds(token);

        foreach (var botId in botIds)
        {
            token.ThrowIfCancellationRequested();
            await Start(botId);
        }
    }
    
    public async Task Start(Guid botId)
    {
        if (!_isWorking)
            return;
        
        try
        {
            var consumeTokenSource = new CancellationTokenSource();
            var bot = await _botReader.Find(botId, consumeTokenSource.Token);
            var client = new TelegramBotClient(bot!.Token);

            await _botConstructor.TrySetup(client, bot, consumeTokenSource.Token);
            
            client.StartReceiving(
                (c, m, t) => _handler.Handle(m, bot.Id, t),
                (c, e, t) => _handler.OnError(e, bot.Id, t),
                ReceiverOptions,
                cancellationToken: consumeTokenSource.Token);
            
            _listeners.TryAdd(botId, consumeTokenSource);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bot {BotId} unhandled exception on start", botId);
        }
    }

    public async Task Restart(Guid botId)
    {
        if (!_isWorking)
            return;
        
        await Stop(botId);
        await Start(botId);
    }

    public async Task Stop(Guid botId)
    {
        if (!_isWorking)
            return;
        
        _listeners.Remove(botId, out var cancellationTokenSource);
        
        if (cancellationTokenSource is null)
            return;
        
        using (cancellationTokenSource)
            await cancellationTokenSource.CancelAsync();
    }

    public async Task StopAsync(CancellationToken token)
    {
        _isWorking = false;

        foreach (var cancellationTokenSource in _listeners.Values)
        {
            token.ThrowIfCancellationRequested();
            
            using (cancellationTokenSource)
                await cancellationTokenSource.CancelAsync();
        }
    }
}