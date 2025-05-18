using Inc.TeamAssistant.Retro.Model.Commands.CreateRetroItem;
using Inc.TeamAssistant.Retro.Model.Commands.MoveToNextRetroState;
using Inc.TeamAssistant.Retro.Model.Commands.StartRetro;
using Inc.TeamAssistant.Retro.Model.Commands.UpdateRetroItem;
using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inc.TeamAssistant.Gateway.Bff;

[ApiController]
[Route("retro")]
[Authorize]
public sealed class RetroController : ControllerBase
{
    private readonly IRetroService _retroService;

    public RetroController(IRetroService retroService)
    {
        _retroService = retroService ?? throw new ArgumentNullException(nameof(retroService));
    }
    
    [HttpGet("items/{teamId:Guid}")]
    public async Task<IActionResult> GetTeam(Guid teamId, CancellationToken token)
    {
        return Ok(await _retroService.GetRetroState(teamId, token));
    }
    
    [HttpPost("items")]
    public async Task<IActionResult> CreateItem([FromBody]CreateRetroItemCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        return Ok(await _retroService.CreateRetroItem(command, CancellationToken.None));
    }
    
    [HttpPut("items")]
    public async Task<IActionResult> UpdateItem([FromBody]UpdateRetroItemCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        await _retroService.UpdateRetroItem(command, CancellationToken.None);
        
        return Ok();
    }
    
    [HttpDelete("items/{retroItemId:Guid}")]
    public async Task<IActionResult> RemoveItem(Guid retroItemId)
    {
        await _retroService.RemoveRetroItem(retroItemId, CancellationToken.None);
        
        return Ok();
    }
    
    [HttpPost]
    public async Task<IActionResult> StartRetro([FromBody]StartRetroCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        return Ok(await _retroService.StartRetro(command, CancellationToken.None));
    }
    
    [HttpPut]
    public async Task<IActionResult> MoveToNextRetroState([FromBody]MoveToNextRetroStateCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        await _retroService.MoveToNextRetroState(command, CancellationToken.None);
        
        return Ok();
    }
}