using System.Security.Claims;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.WebUI.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Person ToPerson(this ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);
        
        var personId = principal.Claims.Single(c => c.Type == nameof(Person.Id));
        var username = principal.Claims.SingleOrDefault(c => c.Type == nameof(Person.Username));
        
        return new Person(long.Parse(personId.Value), principal.Identity!.Name!, username?.Value);
    }
    
    public static ClaimsPrincipal ToClaimsPrincipal(this Person? person)
    {
        if (person is null)
            return new ClaimsPrincipal(new ClaimsIdentity());

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, person.DisplayName),
            new(nameof(Person.Id), person.Id.ToString())
        };
        
        if (!string.IsNullOrWhiteSpace(person.Username))
            claims.Add(new Claim(nameof(Person.Username), person.Username));
        
        var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
        
        return new ClaimsPrincipal(claimsIdentity);
    }
}