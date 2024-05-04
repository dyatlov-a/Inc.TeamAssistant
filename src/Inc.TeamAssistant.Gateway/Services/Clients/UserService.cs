using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.Gateway.Services.Clients;

internal sealed class UserService : IUserService
{
    private readonly AuthService _authService;
    private readonly AuthOptions _authOptions;
    private readonly IBotAccessor _botAccessor;

    public UserService(AuthService authService, AuthOptions authOptions, IBotAccessor botAccessor)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _authOptions = authOptions ?? throw new ArgumentNullException(nameof(authOptions));
        _botAccessor = botAccessor ?? throw new ArgumentNullException(nameof(botAccessor));
    }

    public Task<Person?> GetCurrentUser(CancellationToken token)
    {
        try
        {
            var result = _authService.GetCurrentUser();

            return Task.FromResult(result);
        }
        catch
        {
            return Task.FromResult<Person?>(null);
        }
    }

    public async Task<BotContext> GetAuthBotContext(CancellationToken token)
    {
        var botUserName = await _botAccessor.GetUserName(_authOptions.BotId, token);

        return new BotContext(_authOptions.BotId, botUserName);
    }
}