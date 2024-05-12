using Inc.TeamAssistant.Gateway.Services.Auth;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.WebUI.Contracts;
using Inc.TeamAssistant.WebUI.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Inc.TeamAssistant.Gateway.Controllers;

[ApiController]
[Route("accounts")]
public sealed class AccountsController : ControllerBase
{
    private readonly TelegramAuthService _telegramAuthService;
    private readonly IUserService _userService;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly AuthOptions _authOptions;

    public AccountsController(
        TelegramAuthService telegramAuthService,
        IUserService userService,
        IWebHostEnvironment webHostEnvironment,
        AuthOptions authOptions)
    {
        _telegramAuthService = telegramAuthService ?? throw new ArgumentNullException(nameof(telegramAuthService));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
        _authOptions = authOptions ?? throw new ArgumentNullException(nameof(authOptions));
    }
    
    [HttpGet("bot")]
    public async Task<IActionResult> GetAuthBot() => Ok(await _userService.GetAuthBotContext());

    [HttpGet("current-user")]
    public async Task<IActionResult> GetCurrentUser() => Ok(await _userService.GetCurrentUser());

    [HttpGet("login-tg")]
    public async Task<IActionResult> Login(
        [FromQuery(Name = "id")] long id,
        [FromQuery(Name = "first_name")] string firstName,
        [FromQuery(Name = "last_name")] string? lastName,
        [FromQuery(Name = "username")] string? username,
        [FromQuery(Name = "photo_url")] string? photoUrl,
        [FromQuery(Name = "auth_date")] string authDate,
        [FromQuery(Name = "hash")] string hash)
    {
        if (!await _telegramAuthService.CanLogin(id, firstName, lastName, username, photoUrl, authDate, hash))
            return BadRequest();

        var principal = new Person(id, firstName, username).ToClaimsPrincipal();
        
        await HttpContext.SignInAsync(ApplicationContext.AuthenticationScheme, principal);

        return Redirect("/constructor");
    }
    
    [HttpGet("as-super-user")]
    public async Task<IActionResult> LoginAsSuperUser()
    {
        if (_webHostEnvironment.IsProduction())
            return BadRequest();

        var principal = _authOptions.SuperUser.ToClaimsPrincipal();
        
        await HttpContext.SignInAsync(ApplicationContext.AuthenticationScheme, principal);

        return Redirect("/constructor");
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(ApplicationContext.AuthenticationScheme);

        return Redirect("/");
    }
}