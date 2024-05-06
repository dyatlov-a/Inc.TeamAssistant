using Inc.TeamAssistant.Constructor.Model.Commands.CreateBot;
using Inc.TeamAssistant.Constructor.Model.Commands.UpdateBot;
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

    [HttpGet("features")]
    public async Task<IActionResult> GetFeatures(CancellationToken token)
    {
        return Ok(await _botService.GetFeatures(token));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBotCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var person = User.ToPerson();

        await _botService.Create(
            command with { CurrentUserId = person.Id },
            token);

        return Ok();
    }
    
    [HttpPut]
    public async Task<IActionResult> Update(UpdateBotCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var person = User.ToPerson();

        await _botService.Update(
            command with { CurrentUserId = person.Id },
            token);

        return Ok();
    }

    [HttpDelete("{id:Guid}")]
    public async Task<IActionResult> Remove(Guid id, CancellationToken token)
    {
        var person = User.ToPerson();
        
        await _botService.Remove(id, person.Id, token);

        return Ok();
    }
}