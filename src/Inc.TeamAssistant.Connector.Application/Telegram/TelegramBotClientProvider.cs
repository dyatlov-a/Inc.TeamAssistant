using System.Collections.Concurrent;
using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Primitives.Exceptions;
using Telegram.Bot;

namespace Inc.TeamAssistant.Connector.Application.Telegram;

internal sealed class TelegramBotClientProvider
{
    private readonly ConcurrentDictionary<Guid, TelegramBotClient> _clientMap = new();
    
    private readonly IBotReader _botReader;

    public TelegramBotClientProvider(IBotReader botReader)
    {
        _botReader = botReader ?? throw new ArgumentNullException(nameof(botReader));
    }

    public async Task<ITelegramBotClient> Get(Guid botId, CancellationToken token)
    {
        return await EnsureClientForBot(botId, token);
    }
    
    private async Task<TelegramBotClient> EnsureClientForBot(Guid botId, CancellationToken token)
    {
        if (_clientMap.TryGetValue(botId, out var cachedClient))
            return cachedClient;

        var bot = await _botReader.Find(botId, DateTimeOffset.UtcNow, token);
        if (bot is null)
            throw new TeamAssistantUserException(Messages.Connector_BotNotFound, botId);

        var client = new TelegramBotClient(bot.Token);
        
        _clientMap[bot.Id] = client;

        return client;
    }
}