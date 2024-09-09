using Inc.TeamAssistant.Connector.Model.Commands.RemoveTeammate;
using Inc.TeamAssistant.Constructor.Model.Commands.CreateBot;
using Inc.TeamAssistant.Constructor.Model.Commands.SetBotDetails;
using Inc.TeamAssistant.Constructor.Model.Commands.UpdateBot;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBotDetails;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBotUserName;
using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inc.TeamAssistant.Gateway.Bff;

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
        return Ok(await _botService.GetBotById(id, token));
    }

    [HttpGet("by-user")]
    public async Task<IActionResult> GetByUser(CancellationToken token)
    {
        return Ok(await _botService.GetFromCurrentUser(token));
    }

    [HttpGet("{teamId:guid}/teammates")]
    public async Task<IActionResult> GetTeammates(Guid teamId, CancellationToken token)
    {
        return Ok(await _botService.GetTeammates(teamId, token));
    }
    
    [HttpGet("{teamId:guid}/connector")]
    public async Task<IActionResult> GetConnector(
        Guid teamId,
        [FromQuery]string foreground,
        [FromQuery]string background,
        CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(foreground))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(foreground));
        if (string.IsNullOrWhiteSpace(background))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(background));
        
        return Ok(await _botService.GetConnector(teamId, foreground, background, token));
    }

    [HttpPut("teammate")]
    public async Task<IActionResult> RemoveTeammate(RemoveTeammateCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        await _botService.RemoveTeammate(command, token);
        
        return Ok();
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
    
    [HttpGet("properties")]
    public async Task<IActionResult> GetProperties(CancellationToken token)
    {
        return Ok(await _botService.GetProperties(token));
    }

    [HttpPost("details")]
    public async Task<IActionResult> GetDetails(GetBotDetailsQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        return Ok(await _botService.GetDetails(query, token));
    }

    [HttpPut("set-details")]
    public async Task<IActionResult> SetDetails(SetBotDetailsCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        await _botService.SetDetails(command, token);
        
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBotCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        await _botService.Create(command, token);

        return Ok();
    }
    
    [HttpPut]
    public async Task<IActionResult> Update(UpdateBotCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        await _botService.Update(command, token);

        return Ok();
    }

    [HttpDelete("{id:Guid}")]
    public async Task<IActionResult> Remove(Guid id, CancellationToken token)
    {
        await _botService.Remove(id, token);

        return Ok();
    }
}