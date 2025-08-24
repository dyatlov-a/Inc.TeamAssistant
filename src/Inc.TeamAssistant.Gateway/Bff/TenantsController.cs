using Inc.TeamAssistant.Tenants.Model.Commands.ChangeRoomProperties;
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
    
    [HttpGet("{roomId:Guid}")]
    public async Task<IActionResult> GetTeam(Guid roomId, CancellationToken token)
    {
        return Ok(await _tenantService.GetRoom(roomId, token));
    }
    
    [HttpGet("available/{roomId:Guid?}")]
    public async Task<IActionResult> GetRooms(Guid? roomId, CancellationToken token)
    {
        return Ok(await _tenantService.GetAvailableRooms(roomId, token));
    }
    
    [HttpGet("{roomId:Guid}/properties")]
    public async Task<IActionResult> GetRoomProperties(Guid roomId, CancellationToken token)
    {
        return Ok(await _tenantService.GetRoomProperties(roomId, token));
    }
    
    [HttpGet("{roomId:Guid}/history")]
    public async Task<IActionResult> GetRoomHistory(Guid roomId, CancellationToken token)
    {
        return Ok(await _tenantService.GetRoomHistory(roomId, token));
    }
    
    [HttpPut("properties")]
    public async Task<IActionResult> ChangeRoomProperties([FromBody]ChangeRoomPropertiesCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        await _tenantService.ChangeRoomProperties(command, CancellationToken.None);
        
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> CreateRoom([FromBody]CreateRoomCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        return Ok(await _tenantService.CreateRoom(command, CancellationToken.None));
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateRoom([FromBody]UpdateRoomCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        await _tenantService.UpdateRoom(command, CancellationToken.None);
        
        return Ok();
    }
    
    [HttpDelete("{roomId:Guid}")]
    public async Task<IActionResult> RemoveRoom(Guid roomId)
    {
        await _tenantService.RemoveRoom(roomId, CancellationToken.None);
        
        return Ok();
    }
}