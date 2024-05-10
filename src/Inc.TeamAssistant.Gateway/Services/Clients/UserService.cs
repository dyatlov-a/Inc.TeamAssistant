using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Bots;
using Inc.TeamAssistant.WebUI.Contracts;
using Inc.TeamAssistant.WebUI.Extensions;

namespace Inc.TeamAssistant.Gateway.Services.Clients;

internal sealed class UserService : IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AuthOptions _authOptions;
    private readonly IBotAccessor _botAccessor;

    public UserService(IHttpContextAccessor httpContextAccessor, AuthOptions authOptions, IBotAccessor botAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _authOptions = authOptions ?? throw new ArgumentNullException(nameof(authOptions));
        _botAccessor = botAccessor ?? throw new ArgumentNullException(nameof(botAccessor));
    }

    public Task<Person?> GetCurrentUser(CancellationToken token)
    {
        try
        {
            var result = _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true
                ? _httpContextAccessor.HttpContext.User.ToPerson()
                : null;

            return Task.FromResult(result);
        }
        catch
        {
            return Task.FromResult<Person?>(null);
        }
    }

    public async Task<BotContext> GetAuthBotContext(CancellationToken token)
    {
        return await _botAccessor.GetBotContext(_authOptions.BotId, token);
    }
}