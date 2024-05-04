using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Primitives;
using Telegram.Bot;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class BotAccessor : IBotAccessor
{
    private readonly TelegramBotClientProvider _clientProvider;
    private readonly IBotRepository _botRepository;

    public BotAccessor(TelegramBotClientProvider clientProvider, IBotRepository botRepository)
    {
        _clientProvider = clientProvider ?? throw new ArgumentNullException(nameof(clientProvider));
        _botRepository = botRepository ?? throw new ArgumentNullException(nameof(botRepository));
    }

    public async Task<string> GetUserName(Guid botId, CancellationToken token)
    {
        var client = await _clientProvider.Get(botId, token);

        var botUser = await client.GetMeAsync(token);

        return botUser.Username!;
    }

    public async Task<string> GetToken(Guid botId, CancellationToken token)
    {
        return await _botRepository.GetToken(botId, token);
    }
}