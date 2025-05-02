using Inc.TeamAssistant.Primitives.Bots;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface IUserService
{
    Task<BotContext> GetAuthBotContext(CancellationToken token = default);
}