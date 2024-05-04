using System.Security.Claims;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.WebUI.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static long GetUserId(this ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);
        
        var claim = principal.Claims.Single(c => c.Type == nameof(Person.Id));
        
        return long.Parse(claim.Value);
    }
}