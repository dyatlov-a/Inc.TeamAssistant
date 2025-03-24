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

    public AuthStateProvider(IUserService userService, PersistentComponentState applicationState)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        const string key = nameof(AuthStateProvider);
        var person = _applicationState.TryTakeFromJson<Person>(key, out var restored) && restored is not null
            ? restored
            : await _userService.GetCurrentUser();
        
        _persistingSubscription ??= _applicationState.RegisterOnPersisting(() =>
        {
            _applicationState.PersistAsJson(key, person);
            return Task.CompletedTask;
        });
        
        return new AuthenticationState(person.ToClaimsPrincipal());
    }

    public void Dispose() => _persistingSubscription?.Dispose();
}