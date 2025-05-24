using Inc.TeamAssistant.Retro.Model.Commands.MoveToNextRetroState;
using Inc.TeamAssistant.Retro.Model.Commands.StartRetro;
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
    
    [HttpGet("state/{teamId:Guid}")]
    public async Task<IActionResult> GetRetroState(Guid teamId, CancellationToken token)
    {
        return Ok(await _retroService.GetRetroState(teamId, token));
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