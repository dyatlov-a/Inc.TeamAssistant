using System.Security.Claims;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.WebUI.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Person ToPerson(this ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);

        Claim? personId = null;
        Claim? username = null;

        foreach (var claim in principal.Claims)
            switch (claim.Type)
            {
                case nameof(Person.Id):
                    personId = claim;
                    break;
                case nameof(Person.Username):
                    username = claim;
                    break;
            }

        return new Person(long.Parse(personId!.Value), principal.Identity!.Name!, username?.Value);
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
        
        var claimsIdentity = new ClaimsIdentity(claims, ApplicationContext.AuthenticationScheme);
        
        return new ClaimsPrincipal(claimsIdentity);
    }
}