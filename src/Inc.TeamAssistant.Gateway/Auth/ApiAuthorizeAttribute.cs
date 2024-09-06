using System.Net;
using Inc.TeamAssistant.Gateway.Services.Integrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Inc.TeamAssistant.Gateway.Auth;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
internal sealed class ApiAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var contextProvider = context.HttpContext.RequestServices.GetRequiredService<IntegrationContextProvider>();
        var accessToken = context.HttpContext.Request.Headers["AccessToken"];
        var projectKey = context.HttpContext.Request.Headers["ProjectKey"];
        
        if (string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(projectKey))
            context.Result = new StatusCodeResult((int)HttpStatusCode.Unauthorized);
        else if (!await contextProvider.Ensure(accessToken!, projectKey!))
            context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
    }
}