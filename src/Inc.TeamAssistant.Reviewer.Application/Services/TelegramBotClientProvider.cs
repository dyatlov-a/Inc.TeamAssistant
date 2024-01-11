using Telegram.Bot;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class TelegramBotClientProvider
{
    private readonly TelegramBotClient _client;

    public TelegramBotClientProvider(string accessToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(accessToken));
        
        _client = new(accessToken);
    }

    public ITelegramBotClient Get() => _client;
}