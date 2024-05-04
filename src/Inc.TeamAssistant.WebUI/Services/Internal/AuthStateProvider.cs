using System.Security.Claims;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.WebUI.Contracts;
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
        
        if (person is null)
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, person.DisplayName),
            new(nameof(Person.Id), person.Id.ToString())
        };
        
        if (!string.IsNullOrWhiteSpace(person.Username))
            claims.Add(new Claim(nameof(Person.Username), person.Username));
        
        var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        var result = new AuthenticationState(claimsPrincipal);

        return result;
    }
}