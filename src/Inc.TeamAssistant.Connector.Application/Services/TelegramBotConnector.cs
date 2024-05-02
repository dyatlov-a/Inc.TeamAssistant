using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Primitives;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class TelegramBotConnector : IHostedService
{
    private readonly TelegramBotMessageHandler _handler;
    private readonly IBotRepository _botRepository;
    private readonly BotConstructor _botConstructor;
    private readonly IBotAccessor _botAccessor;
    private readonly ILogger<TelegramBotConnector> _logger;
    
    public TelegramBotConnector(
        TelegramBotMessageHandler handler,
        IBotRepository botRepository,
        BotConstructor botConstructor,
        IBotAccessor botAccessor,
        ILogger<TelegramBotConnector> logger)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        _botRepository = botRepository ?? throw new ArgumentNullException(nameof(botRepository));
        _botConstructor = botConstructor ?? throw new ArgumentNullException(nameof(botConstructor));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _botAccessor = botAccessor ?? throw new ArgumentNullException(nameof(botAccessor));
    }

    public async Task StartAsync(CancellationToken token)
    {
        var botIds = await _botRepository.GetBotIds(token);

        foreach (var botId in botIds)
            await Start(botId, token);
    }
    
    private async Task Start(Guid botId, CancellationToken token)
    {
        try
        {
            var bot = await _botRepository.Find(botId, token);
            var client = new TelegramBotClient(bot!.Token);

            await _botConstructor.TrySetup(client, bot, token);
            
            var userName = await _botAccessor.GetUserName(botId, token);
            var botContext = new BotContext(bot.Id, userName);

            client.StartReceiving(
                (c, m, t) => _handler.Handle(m, botContext, t),
                (c, e, t) => _handler.OnError(e, botContext, t),
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