using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.WebUI.Contracts;
using Inc.TeamAssistant.WebUI.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Inc.TeamAssistant.WebUI.Services.Internal;

internal sealed class AuthStateProvider : AuthenticationStateProvider, IDisposable
{
    private readonly IUserService _userService;
    private readonly PersistentComponentState _applicationState;
    private PersistingComponentStateSubscription? _persistingSubscription;
    private static readonly string Key = nameof(AuthStateProvider);

    public AuthStateProvider(IUserService userService, PersistentComponentState applicationState)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        _persistingSubscription ??= _applicationState.RegisterOnPersisting(Persist);
        
        var person = _applicationState.TryTakeFromJson<Person>(Key, out var restored) && restored is not null
            ? restored
            : await _userService.GetCurrentUser();
        
        return new AuthenticationState(person.ToClaimsPrincipal());
    }

    private async Task Persist()
    {
        var person = await _userService.GetCurrentUser();
        _applicationState.PersistAsJson(Key, person);
    }

    public void Dispose() => _persistingSubscription?.Dispose();
}