using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inc.TeamAssistant.Gateway.Controllers;

[ApiController]
[Route("bots")]
[Authorize]
public sealed class BotsController : ControllerBase
{
    private readonly IBotService _botService;

    public BotsController(IBotService botService)
    {
        _botService = botService ?? throw new ArgumentNullException(nameof(botService));
    }

    [HttpGet("{ownerId}")]
    public async Task<IActionResult> Get(long ownerId, CancellationToken token)
    {
        return Ok(await _botService.GetBotsByOwner(ownerId, token));
    }
}