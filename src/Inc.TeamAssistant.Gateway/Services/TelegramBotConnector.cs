using Telegram.Bot;

namespace Inc.TeamAssistant.Gateway.Services;

internal sealed class TelegramBotConnector : IHostedService
{
    private readonly TelegramBotMessageHandler _handler;
    private readonly TelegramBotClient _client;

    public TelegramBotConnector(TelegramBotMessageHandler handler, string accessToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(accessToken));

		_handler = handler ?? throw new ArgumentNullException(nameof(handler));
        _client = new(accessToken);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _client.StartReceiving(_handler.Handle, _handler.OnError, cancellationToken: cancellationToken);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}