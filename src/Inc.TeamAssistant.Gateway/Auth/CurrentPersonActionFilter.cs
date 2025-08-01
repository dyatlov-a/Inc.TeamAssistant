using Inc.TeamAssistant.Gateway.Services.ServerCore;
using Inc.TeamAssistant.WebUI.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Inc.TeamAssistant.Gateway.Auth;

internal sealed class CurrentPersonActionFilter : IActionFilter
{
    private readonly PersonResolver _personResolver;

    public CurrentPersonActionFilter(PersonResolver personResolver)
    {
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
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