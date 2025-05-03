using Inc.TeamAssistant.Gateway.Configs;
using Inc.TeamAssistant.Primitives.Bots;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.Gateway.Services.Clients;

internal sealed class UserService : IUserService
{
    private readonly AuthOptions _authOptions;
    private readonly IBotAccessor _botAccessor;

    public UserService(AuthOptions authOptions, IBotAccessor botAccessor)
    {
        _authOptions = authOptions ?? throw new ArgumentNullException(nameof(authOptions));
        _botAccessor = botAccessor ?? throw new ArgumentNullException(nameof(botAccessor));
    }

    public async Task<BotContext> GetAuthBotContext(CancellationToken token)
    {
        return await _botAccessor.GetBotContext(_authOptions.BotId, token);
    }
}