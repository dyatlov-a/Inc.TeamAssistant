using Inc.TeamAssistant.Tenants.Model.Commands.CreateTeam;
using Inc.TeamAssistant.Tenants.Model.Commands.UpdateTeam;
using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inc.TeamAssistant.Gateway.Bff;

[ApiController]
[Route("tenants/teams")]
[Authorize]
public sealed class TenantsController : ControllerBase
{
    private readonly ITenantService _tenantService;

    public TenantsController(ITenantService tenantService)
    {
        _tenantService = tenantService ?? throw new ArgumentNullException(nameof(tenantService));
    }
    
    [HttpGet("{teamId:Guid}")]
    public async Task<IActionResult> GetTeam(Guid teamId, CancellationToken token)
    {
        return Ok(await _tenantService.GetTeam(teamId, token));
    }
    
    [HttpGet("available/{teamId:Guid?}")]
    public async Task<IActionResult> GetTeams(Guid? teamId, CancellationToken token)
    {
        return Ok(await _tenantService.GetAvailableTeams(teamId, token));
    }

    [HttpPost]
    public async Task<IActionResult> CreateTeam([FromBody]CreateTeamCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        return Ok(await _tenantService.CreateTeam(command, CancellationToken.None));
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateTeam([FromBody]UpdateTeamCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        await _tenantService.UpdateTeam(command, CancellationToken.None);
        
        return Ok();
    }
    
    [HttpDelete("{teamId:Guid}")]
    public async Task<IActionResult> RemoveTeam(Guid teamId)
    {
        await _tenantService.RemoveTeam(teamId, CancellationToken.None);
        
        return Ok();
    }
}