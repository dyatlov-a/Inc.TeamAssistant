using System.Security.Claims;
using Inc.TeamAssistant.Primitives;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Telegram.Bot.Extensions.LoginWidget;

namespace Inc.TeamAssistant.Gateway.Services;

public sealed class AuthService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AuthOptions _options;
    private readonly IBotAccessor _botAccessor;

    public AuthService(IHttpContextAccessor httpContextAccessor, AuthOptions options, IBotAccessor botAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _botAccessor = botAccessor ?? throw new ArgumentNullException(nameof(botAccessor));
    }
    
    public Person? GetCurrentUser()
    {
        if (_httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated != true)
            return null;

        var user = _httpContextAccessor.HttpContext!.User;
        var personId = user.Claims.Single(c => c.Type == nameof(Person.Id));
        var username = user.Claims.SingleOrDefault(c => c.Type == nameof(Person.Username));

        return new Person(long.Parse(personId.Value), user.Identity.Name!, username?.Value);
    }

    public async Task<bool> Login(
        long id,
        string firstName,
        string? lastName,
        string? username,
        string? photoUrl,
        string authDate,
        string hash)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(firstName));
        if (string.IsNullOrWhiteSpace(authDate))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(authDate));
        if (string.IsNullOrWhiteSpace(hash))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(hash));

        var fields = Build(id, firstName, lastName, username, photoUrl, authDate, hash);
        var token = await _botAccessor.GetToken(_options.BotId);
        var loginWidget = new LoginWidget(token);
        var authState = loginWidget.CheckAuthorization(fields);

        if (authState != Authorization.Valid)
            return false;
        
        await _httpContextAccessor.HttpContext!.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            CreatePrincipal(new Person(id, firstName, username)));

        return true;
    }

    public async Task Logout()
    {
        await _httpContextAccessor.HttpContext!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    private Dictionary<string, string> Build(
        long id,
        string firstName,
        string? lastName,
        string? username,
        string? photoUrl,
        string authDate,
        string hash)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(firstName));
        if (string.IsNullOrWhiteSpace(authDate))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(authDate));
        if (string.IsNullOrWhiteSpace(hash))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(hash));
        
        var fields = new Dictionary<string, string>
        {
            ["id"] = id.ToString(),
            ["first_name"] = firstName
        };
        
        if (lastName is not null)
            fields.Add("last_name", lastName);
        if (lastName is not null)
            fields.Add("username", username!);
        if (lastName is not null)
            fields.Add("photo_url", photoUrl!);
        
        fields.Add("auth_date", authDate);
        fields.Add("hash", hash);

        return fields;
    }
    
    private ClaimsPrincipal CreatePrincipal(Person person)
    {
        ArgumentNullException.ThrowIfNull(person);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, person.DisplayName),
            new(nameof(Person.Id), person.Id.ToString())
        };
        
        if (!string.IsNullOrWhiteSpace(person.Username))
            claims.Add(new Claim(nameof(Person.Username), person.Username));
        
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        
        return new ClaimsPrincipal(new[] { claimsIdentity });
    }
}