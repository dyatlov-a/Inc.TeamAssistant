using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Primitives.Bots;
using Inc.TeamAssistant.Primitives.Exceptions;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class BotAccessor : IBotAccessor
{
    private readonly IBotReader _botReader;

    public BotAccessor(IBotReader botReader)
    {
        _botReader = botReader ?? throw new ArgumentNullException(nameof(botReader));
    }

    public async Task<BotContext> GetBotContext(Guid botId, CancellationToken token)
    {
        var bot = await _botReader.Find(botId, token);
        if (bot is null)
            throw new TeamAssistantUserException(Messages.Connector_BotNotFound, botId);

        return new BotContext(bot.Id, bot.Name, bot.Properties);
    }

    public async Task<string> GetToken(Guid botId, CancellationToken token)
    {
        return await _botReader.GetToken(botId, token);
    }
}