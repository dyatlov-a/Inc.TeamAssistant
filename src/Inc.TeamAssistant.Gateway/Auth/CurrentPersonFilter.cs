using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.WebUI.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.SignalR;

namespace Inc.TeamAssistant.Gateway.Auth;

internal sealed class CurrentPersonFilter : IHubFilter, IActionFilter
{
    private readonly IPersonResolver _personResolver;

    public CurrentPersonFilter(IPersonResolver personResolver)
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

    public void OnActionExecuting(ActionExecutingContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        
        var user = context.HttpContext.User;
        
        if (user.Identity?.IsAuthenticated == true)
            _personResolver.TrySet(user.ToPerson());
        else
            _personResolver.Reset();
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        _personResolver.Reset();
    }
}