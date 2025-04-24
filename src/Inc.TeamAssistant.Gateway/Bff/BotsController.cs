using Inc.TeamAssistant.Connector.Model.Commands.UpdateWidgets;
using Inc.TeamAssistant.Constructor.Model.Commands.CreateBot;
using Inc.TeamAssistant.Constructor.Model.Commands.SetBotDetails;
using Inc.TeamAssistant.Constructor.Model.Commands.UpdateBot;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBotDetails;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBotUserName;
using Inc.TeamAssistant.Reviewer.Model.Commands.RemoveTeammate;
using Inc.TeamAssistant.Reviewer.Model.Commands.UpdateTeammate;
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
    
    [HttpGet("{botId:guid}/widgets")]
    public async Task<IActionResult> Widgets(Guid botId, CancellationToken token)
    {
        return Ok(await _botService.GetWidgetsForCurrentUser(botId, token));
    }
    
    [HttpPut("widgets")]
    public async Task<IActionResult> Widgets(UpdateWidgetsCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        await _botService.UpdateWidgets(command, CancellationToken.None);

        return Ok();
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
        ArgumentException.ThrowIfNullOrWhiteSpace(foreground);
        ArgumentException.ThrowIfNullOrWhiteSpace(background);
        
        return Ok(await _botService.GetConnector(teamId, foreground, background, token));
    }

    [HttpPost("teammate")]
    public async Task<IActionResult> RemoveTeammate(RemoveTeammateCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        await _botService.RemoveTeammate(command, CancellationToken.None);
        
        return Ok();
    }
    
    [HttpPut("teammate")]
    public async Task<IActionResult> UpdateTeammate(UpdateTeammateCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        await _botService.UpdateTeammate(command, CancellationToken.None);
        
        return Ok();
    }

    [HttpPost("check")]
    public async Task<IActionResult> Check(GetBotUserNameQuery query)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        return Ok(await _botService.Check(query, CancellationToken.None));
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
    public async Task<IActionResult> GetDetails(GetBotDetailsQuery query)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        return Ok(await _botService.GetDetails(query, CancellationToken.None));
    }

    [HttpPut("set-details")]
    public async Task<IActionResult> SetDetails(SetBotDetailsCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        await _botService.SetDetails(command, CancellationToken.None);
        
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBotCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        await _botService.Create(command, CancellationToken.None);

        return Ok();
    }
    
    [HttpPut]
    public async Task<IActionResult> Update(UpdateBotCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        await _botService.Update(command, CancellationToken.None);

        return Ok();
    }

    [HttpDelete("{id:Guid}")]
    public async Task<IActionResult> Remove(Guid id)
    {
        await _botService.Remove(id, CancellationToken.None);

        return Ok();
    }
}