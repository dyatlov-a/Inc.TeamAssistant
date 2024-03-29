using Inc.TeamAssistant.Connector.Application.Contracts;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class TelegramBotConnector : IHostedService
{
    private readonly TelegramBotMessageHandler _handler;
    private readonly IBotRepository _botRepository;
    
    public TelegramBotConnector(TelegramBotMessageHandler handler, IBotRepository botRepository)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        _botRepository = botRepository ?? throw new ArgumentNullException(nameof(botRepository));
    }

    public async Task StartAsync(CancellationToken token)
    {
        var bots = await _botRepository.GetAll(token);

        foreach (var bot in bots)
        {
            var client = new TelegramBotClient(bot.Token);

            client.StartReceiving(
                (c, m, t) => _handler.Handle(m, bot.Id, t),
                (c, e, t) => _handler.OnError(e, bot.Name, t),
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
    }

    public Task StopAsync(CancellationToken token) => Task.CompletedTask;
}