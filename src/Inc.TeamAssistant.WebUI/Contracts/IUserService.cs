using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface IUserService
{
    Task<Person?> GetCurrentUser(CancellationToken token = default);
    
    Task<BotContext> GetAuthBotContext(CancellationToken token = default);
}