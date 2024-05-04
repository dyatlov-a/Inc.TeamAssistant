using Inc.TeamAssistant.Connector.Application.Contracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class TelegramBotConnector : IHostedService
{
    private readonly TelegramBotMessageHandler _handler;
    private readonly IBotReader _botReader;
    private readonly BotConstructor _botConstructor;
    private readonly ILogger<TelegramBotConnector> _logger;
    
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
        var botIds = await _botReader.GetBotIds(token);

        foreach (var botId in botIds)
            await Start(botId, token);
    }
    
    private async Task Start(Guid botId, CancellationToken token)
    {
        try
        {
            var bot = await _botReader.Find(botId, token);
            var client = new TelegramBotClient(bot!.Token);

            await _botConstructor.TrySetup(client, bot, token);

            client.StartReceiving(
                (c, m, t) => _handler.Handle(m, bot.Id, t),
                (c, e, t) => _handler.OnError(e, bot.Id, t),
                receiverOptions: new ReceiverOptions
                {
                    AllowedUpdates =
                    [
                        UpdateType.Message,
                        UpdateType.CallbackQuery,
                        UpdateType.PollAnswer
                    ]
                },
                cancellationToken: token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bot {BotId} unhandled exception on start", botId);
        }
    }

    public Task StopAsync(CancellationToken token) => Task.CompletedTask;
}