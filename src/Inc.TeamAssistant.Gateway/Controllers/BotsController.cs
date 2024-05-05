using Inc.TeamAssistant.Constructor.Model.Queries.GetBotUserName;
using Inc.TeamAssistant.WebUI.Contracts;
using Inc.TeamAssistant.WebUI.Extensions;
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

    [HttpGet("{id:Guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken token)
    {
        var person = User.ToPerson();
        var getBotByIdResult = await _botService.GetBotById(id, person.Id, token);
        
        return getBotByIdResult.Result is null
            ? NotFound()
            : Ok(getBotByIdResult);
    }

    [HttpGet("{ownerId:long}")]
    public async Task<IActionResult> Get(long ownerId, CancellationToken token)
    {
        return Ok(await _botService.GetBotsByOwner(ownerId, token));
    }

    [HttpPost("check")]
    public async Task<IActionResult> Check(GetBotUserNameQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        return Ok(await _botService.Check(query, token));
    }
}