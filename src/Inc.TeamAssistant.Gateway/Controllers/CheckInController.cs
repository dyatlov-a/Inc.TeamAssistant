using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Inc.TeamAssistant.Gateway.Controllers;

[ApiController]
[Route("check-in")]
public sealed class CheckInController : ControllerBase
{
    private readonly ICheckInService _service;

    public CheckInController(ICheckInService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    [HttpGet("maps/{botId}")]
    public async Task<IActionResult> GetMaps(Guid botId, CancellationToken token)
    {
        return Ok(await _service.GetMaps(botId, token));
    }

    [HttpGet("locations/{mapId}")]
    public async Task<IActionResult> GetStoryDetails(Guid mapId, CancellationToken token)
    {
        return Ok(await _service.GetLocations(mapId, token));
    }
}