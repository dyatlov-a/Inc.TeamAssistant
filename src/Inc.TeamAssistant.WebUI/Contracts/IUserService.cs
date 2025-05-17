using Inc.TeamAssistant.WebUI.Features.Auth;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface IUserService
{
    Task<AuthBotContext> GetAuthBotContext(CancellationToken token = default);
}