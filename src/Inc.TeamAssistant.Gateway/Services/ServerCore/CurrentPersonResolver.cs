using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.WebUI.Extensions;

namespace Inc.TeamAssistant.Gateway.Services.ServerCore;

internal sealed class CurrentPersonResolver : ICurrentPersonResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentPersonResolver(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public Person GetCurrentPerson()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
            throw new ApplicationException("Process has no access to the HTTP context");
        
        var person = httpContext.User.ToPerson();
        return person;
    }
}