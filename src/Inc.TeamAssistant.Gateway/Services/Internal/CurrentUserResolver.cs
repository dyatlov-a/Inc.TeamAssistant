using Inc.TeamAssistant.Constructor.Application.Contracts;
using Inc.TeamAssistant.WebUI.Extensions;

namespace Inc.TeamAssistant.Gateway.Services.Internal;

internal sealed class CurrentUserResolver : ICurrentUserResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserResolver(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public long GetUserId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
            throw new ApplicationException("Process has no access to the HTTP context");
        
        var person = httpContext.User.ToPerson();
        return person.Id;
    }
}