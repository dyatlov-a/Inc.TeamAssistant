using Inc.TeamAssistant.Tenants.Model.Commands.CreateTeam;
using Inc.TeamAssistant.Tenants.Model.Commands.UpdateTeam;
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
    
    [HttpGet("team/{teamId:Guid}")]
    public async Task<IActionResult> GetTeam(Guid teamId, CancellationToken token)
    {
        return Ok(await _retroService.GetTeam(teamId, token));
    }

    [HttpPost("team")]
    public async Task<IActionResult> CreateTeam([FromBody]CreateTeamCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        return Ok(await _retroService.CreateTeam(command, CancellationToken.None));
    }
    
    [HttpPut("team")]
    public async Task<IActionResult> UpdateTeam([FromBody]UpdateTeamCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        await _retroService.UpdateTeam(command, CancellationToken.None);
        
        return Ok();
    }
}