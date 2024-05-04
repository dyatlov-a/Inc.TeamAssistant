using Inc.TeamAssistant.WebUI.Contracts;
using Inc.TeamAssistant.WebUI.Extensions;
using Microsoft.AspNetCore.Components.Authorization;

namespace Inc.TeamAssistant.WebUI.Services.Internal;

internal sealed class AuthStateProvider : AuthenticationStateProvider
{
    private readonly IUserService _userService;

    public AuthStateProvider(IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var person = await _userService.GetCurrentUser();

        return new AuthenticationState(person.ToClaimsPrincipal());
    }
}