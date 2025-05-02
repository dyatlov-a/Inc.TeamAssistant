using Inc.TeamAssistant.Gateway.Configs;
using Inc.TeamAssistant.Gateway.Services.ServerCore;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.WebUI.Contracts;
using Inc.TeamAssistant.WebUI.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Inc.TeamAssistant.Gateway.Bff;

[ApiController]
[Route("accounts")]
public sealed class AccountsController : ControllerBase
{
    private readonly TelegramAuthService _telegramAuthService;
    private readonly IUserService _userService;
    private readonly AuthOptions _authOptions;

    public AccountsController(
        TelegramAuthService telegramAuthService,
        IUserService userService,
        AuthOptions authOptions)
    {
        _telegramAuthService = telegramAuthService ?? throw new ArgumentNullException(nameof(telegramAuthService));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _authOptions = authOptions ?? throw new ArgumentNullException(nameof(authOptions));
    }
    
    [HttpGet("bot")]
    public async Task<IActionResult> GetAuthBot() => Ok(await _userService.GetAuthBotContext());

    [HttpGet("login-tg")]
    public async Task<IActionResult> Login(
        [FromQuery(Name = "id")] long id,
        [FromQuery(Name = "first_name")] string firstName,
        [FromQuery(Name = "auth_date")] string authDate,
        [FromQuery(Name = "hash")] string hash,
        [FromQuery(Name = "last_name")] string? lastName,
        [FromQuery(Name = "username")] string? username,
        [FromQuery(Name = "photo_url")] string? photoUrl,
        [FromQuery(Name = "return_url")] string? returnUrl)
    {
        if (!await _telegramAuthService.CanLogin(id, firstName, lastName, username, photoUrl, authDate, hash))
            return BadRequest();

        var principal = new Person(id, firstName, username).ToClaimsPrincipal();
        
        await HttpContext.SignInAsync(ApplicationContext.AuthenticationScheme, principal);
        
        return Redirect(DetectTargetUrl(returnUrl));
    }
    
    [HttpGet("login-as-super-user")]
    public async Task<IActionResult> LoginAsSuperUser(string? returnUrl)
    {
        if (_authOptions.SuperUser is null)
            return NotFound();

        var principal = _authOptions.SuperUser.ToClaimsPrincipal();
        
        await HttpContext.SignInAsync(ApplicationContext.AuthenticationScheme, principal);
        
        return Redirect(DetectTargetUrl(returnUrl));
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout(string? languageCode)
    {
        await HttpContext.SignOutAsync(ApplicationContext.AuthenticationScheme);

        var path = string.IsNullOrWhiteSpace(languageCode)
            ? "/"
            : $"/{languageCode}/";
        
        return Redirect(path);
    }

    private string DetectTargetUrl(string? returnUrl)
    {
        if (string.IsNullOrWhiteSpace(returnUrl))
            return "/constructor";

        var returnUri = new Uri(returnUrl);
        var returnUrlHost = returnUri.IsDefaultPort
            ? returnUri.Host
            : $"{returnUri.Host}:{returnUri.Port}";

        return string.IsNullOrWhiteSpace(returnUrlHost) || returnUrlHost == Request.Host.ToUriComponent()
            ? returnUrl
            : "/constructor";
    }
}