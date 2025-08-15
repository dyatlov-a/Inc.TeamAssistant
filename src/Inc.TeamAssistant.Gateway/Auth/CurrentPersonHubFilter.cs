using Inc.TeamAssistant.Gateway.Services.ServerCore;
using Inc.TeamAssistant.WebUI.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace Inc.TeamAssistant.Gateway.Auth;

internal sealed class CurrentPersonHubFilter : IHubFilter
{
    private readonly PersonResolver _personResolver;

    public CurrentPersonHubFilter(PersonResolver personResolver)
    {
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
    }
    
    public ValueTask<object?> InvokeMethodAsync(
        HubInvocationContext invocationContext,
        Func<HubInvocationContext, ValueTask<object?>> next)
    {
        var user = invocationContext.Context.User;
        
        if (user is not null)
            _personResolver.TrySet(user.ToPerson());
        
        return next(invocationContext);
    }
}