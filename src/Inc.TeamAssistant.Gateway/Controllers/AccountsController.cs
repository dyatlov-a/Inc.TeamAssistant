using Inc.TeamAssistant.Gateway.Services;
using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Inc.TeamAssistant.Gateway.Controllers;

[ApiController]
[Route("accounts")]
public sealed class AccountsController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly IUserService _userService;

    public AccountsController(AuthService authService, IUserService userService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }
    
    [HttpGet("bot")]
    public async Task<IActionResult> GetAuthBot() => Ok(await _userService.GetAuthBotContext());

    [HttpGet("current-user")]
    public async Task<IActionResult> GetCurrentUser() => Ok(await _userService.GetCurrentUser());

    [HttpGet("login_tg")]
    public async Task<IActionResult> Login(
        [FromQuery(Name = "id")] long id,
        [FromQuery(Name = "first_name")] string firstName,
        [FromQuery(Name = "last_name")] string? lastName,
        [FromQuery(Name = "username")] string? username,
        [FromQuery(Name = "photo_url")] string? photoUrl,
        [FromQuery(Name = "auth_date")] string authDate,
        [FromQuery(Name = "hash")] string hash)
    {
        return await _authService.Login(id, firstName, lastName, username, photoUrl, authDate, hash)
            ? Redirect("/constructor")
            : BadRequest();
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await _authService.Logout();

        return Redirect("/");
    }
}