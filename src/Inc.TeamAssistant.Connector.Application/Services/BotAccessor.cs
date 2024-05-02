using Inc.TeamAssistant.Primitives;
using Telegram.Bot;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class BotAccessor : IBotAccessor
{
    private readonly TelegramBotClientProvider _clientProvider;

    public BotAccessor(TelegramBotClientProvider clientProvider)
    {
        _clientProvider = clientProvider ?? throw new ArgumentNullException(nameof(clientProvider));
    }

    public async Task<string> GetUserName(Guid botId, CancellationToken token)
    {
        var client = await _clientProvider.Get(botId, token);

        var botUser = await client.GetMeAsync(token);

        return botUser.Username!;
    }
}