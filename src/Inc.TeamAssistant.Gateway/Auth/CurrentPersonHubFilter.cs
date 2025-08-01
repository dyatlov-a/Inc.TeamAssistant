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

    public Task OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);
        
        var user = context.Context.User;
        
        if (user is not null)
            _personResolver.TrySet(user.ToPerson());
        else
            _personResolver.Reset();
        
        return next(context);
    }
    
    public Task OnDisconnectedAsync(
        HubLifetimeContext context,
        Exception? exception,
        Func<HubLifetimeContext, Exception?, Task> next)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);
        
        _personResolver.Reset();
        
        return next(context, exception);
    }
}