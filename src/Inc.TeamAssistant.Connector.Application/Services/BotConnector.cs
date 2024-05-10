using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Bots;
using Telegram.Bot;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class BotConnector : IBotConnector
{
    public async Task<string> CheckAccess(string botToken, CancellationToken token)
    {
        try
        {
            var client = new TelegramBotClient(botToken);

            var bot = await client.GetMeAsync(token);

            return bot.Username ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }
}