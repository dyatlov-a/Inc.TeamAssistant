using Inc.TeamAssistant.Tenants.Model.Commands.CreateRoom;
using Inc.TeamAssistant.Tenants.Model.Commands.UpdateRoom;
using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inc.TeamAssistant.Gateway.Bff;

[ApiController]
[Route("tenants/rooms")]
[Authorize]
public sealed class TenantsController : ControllerBase
{
    private readonly ITenantService _tenantService;

    public TenantsController(ITenantService tenantService)
    {
        _tenantService = tenantService ?? throw new ArgumentNullException(nameof(tenantService));
    }
    
    [HttpGet("{roomId:Guid}/properties")]
    public async Task<IActionResult> GetRoomProperties(Guid roomId, CancellationToken token)
    {
        return Ok(await _tenantService.GetRoomProperties(roomId, token));
    }
    
    [HttpGet("{roomId:Guid}")]
    public async Task<IActionResult> GetTeam(Guid roomId, CancellationToken token)
    {
        return Ok(await _tenantService.GetRoom(roomId, token));
    }
    
    [HttpGet("{roomId:Guid?}/available")]
    public async Task<IActionResult> GetTeams(Guid? roomId, CancellationToken token)
    {
        return Ok(await _tenantService.GetAvailableRooms(roomId, token));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody]CreateRoomCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        return Ok(await _tenantService.CreateRoom(command, CancellationToken.None));
    }
    
    [HttpPut]
    public async Task<IActionResult> Update([FromBody]UpdateRoomCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        await _tenantService.UpdateRoom(command, CancellationToken.None);
        
        return Ok();
    }
    
    [HttpDelete("{roomId:Guid}")]
    public async Task<IActionResult> Remove(Guid roomId)
    {
        await _tenantService.RemoveRoom(roomId, CancellationToken.None);
        
        return Ok();
    }
}