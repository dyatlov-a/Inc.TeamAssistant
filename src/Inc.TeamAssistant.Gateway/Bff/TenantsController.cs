using Inc.TeamAssistant.Tenants.Model.Commands.CreateRoom;
using Inc.TeamAssistant.Tenants.Model.Commands.UpdateRoom;
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
        return Ok(await _tenantService.GetRoom(teamId, token));
    }
    
    [HttpGet("available/{teamId:Guid?}")]
    public async Task<IActionResult> GetTeams(Guid? teamId, CancellationToken token)
    {
        return Ok(await _tenantService.GetAvailableRooms(teamId, token));
    }

    [HttpPost]
    public async Task<IActionResult> CreateTeam([FromBody]CreateRoomCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        return Ok(await _tenantService.CreateRoom(command, CancellationToken.None));
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateTeam([FromBody]UpdateRoomCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        await _tenantService.UpdateRoom(command, CancellationToken.None);
        
        return Ok();
    }
    
    [HttpDelete("{teamId:Guid}")]
    public async Task<IActionResult> RemoveTeam(Guid teamId)
    {
        await _tenantService.RemoveRoom(teamId, CancellationToken.None);
        
        return Ok();
    }
}