using Inc.TeamAssistant.Gateway.Auth;
using Inc.TeamAssistant.Gateway.Configs;
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
        [FromQuery(Name = TelegramAuthProperties.Profile.Id)] long id,
        [FromQuery(Name = TelegramAuthProperties.Profile.FirstName)] string firstName,
        [FromQuery(Name = TelegramAuthProperties.Profile.AuthDate)] string authDate,
        [FromQuery(Name = TelegramAuthProperties.Profile.Hash)] string hash,
        [FromQuery(Name = TelegramAuthProperties.Profile.LastName)] string? lastName,
        [FromQuery(Name = TelegramAuthProperties.Profile.Username)] string? username,
        [FromQuery(Name = TelegramAuthProperties.Profile.PhotoUrl)] string? photoUrl,
        [FromQuery(Name = TelegramAuthProperties.ReturnUrl)] string? returnUrl)
    {
        var person = await _telegramAuthService.CanLoginAs(
            id,
            firstName,
            lastName,
            username,
            photoUrl,
            authDate,
            hash);
        
        if (person is null)
            return BadRequest();
        
        await HttpContext.SignInAsync(ApplicationContext.AuthenticationScheme, person.ToClaimsPrincipal());
        
        return Redirect(DetectTargetUrl(returnUrl));
    }
    
    [HttpGet("login-as-system-user/{personId}")]
    public async Task<IActionResult> LoginAsSystemUser(long personId, string? returnUrl)
    {
        var systemUser = _authOptions.SystemUsers.SingleOrDefault(s => s.Id == personId);
        if (systemUser is null)
            return NotFound();

        var principal = systemUser.ToClaimsPrincipal();
        
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
        const string constructorPage = "/constructor";
        
        if (string.IsNullOrWhiteSpace(returnUrl))
            return constructorPage;

        var returnUri = new Uri(returnUrl);
        var returnUrlHost = returnUri.IsDefaultPort
            ? returnUri.Host
            : $"{returnUri.Host}:{returnUri.Port}";

        return string.IsNullOrWhiteSpace(returnUrlHost) || returnUrlHost == Request.Host.ToUriComponent()
            ? returnUrl
            : constructorPage;
    }
}